using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Moderation.Entities;
using SNS.Domain.Moderation.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Moderation.Commands.ReportProject;

public sealed record ReportProjectCommand(
    Guid ProjectId,
    ViolationReason ViolationReason,
    string? AdditionalDetails
) : ICommand<Guid>;

internal sealed class ReportProjectCommandHandler : ICommandHandler<ReportProjectCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<ReportTicket> _ticketRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ReportProjectCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<ReportTicket> ticketRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _ticketRepo = ticketRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Guid>> Handle(ReportProjectCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var projectExists = await _dbContext.Projects.AnyAsync(p => p.Id == request.ProjectId && p.IsActive, cancellationToken);
        if (!projectExists)
        {
            return Result<Guid>.Failure(ResourceStatusCode.NotFound);
        }

        var ticket = await _ticketRepo.GetSingleByExpressionAsync(
            t => t.TargetId == request.ProjectId && t.TargetType == ReportTargetType.Project, 
            cancellationToken);

        if (ticket == null)
        {
            ticket = ReportTicket.Create(request.ProjectId, ReportTargetType.Project);
            _ticketRepo.Add(ticket);
        }
        else
        {
            var alreadyReported = await _dbContext.ContentReports.AnyAsync(
                r => r.TicketId == ticket.Id && r.ReporterId == currentUserId.Value,
                cancellationToken);

            if (alreadyReported)
            {
                return Result<Guid>.Failure(OperationStatusCode.Conflict);
            }
        }

        var report = ContentReport.Create(ticket.Id, currentUserId.Value, request.ViolationReason, request.AdditionalDetails);
        ticket.AddReport(report);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(ticket.Id, OperationStatusCode.Success);
    }
}

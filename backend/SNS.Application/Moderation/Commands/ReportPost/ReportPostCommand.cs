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

namespace SNS.Application.Moderation.Commands.ReportPost;

public sealed record ReportPostCommand(
    Guid PostId,
    ViolationReason Reason,
    string? Details
) : ICommand;

internal sealed class ReportPostCommandHandler : ICommandHandler<ReportPostCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<ReportTicket> _ticketRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ReportPostCommandHandler(
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

    public async Task<Result> Handle(ReportPostCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var postExists = await _dbContext.Posts.AnyAsync(p => p.Id == request.PostId && p.IsActive, cancellationToken);
        if (!postExists)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var ticket = await _ticketRepo.GetSingleByExpressionAsync(
            t => t.TargetId == request.PostId && t.TargetType == ReportTargetType.Post, 
            cancellationToken);

        if (ticket == null)
        {
            ticket = ReportTicket.Create(request.PostId, ReportTargetType.Post);
            _ticketRepo.Add(ticket);
        }
        else
        {
            // Add report to existing ticket
        }

        var report = ContentReport.Create(profileId.Value, request.Reason, request.Details);
        ticket.AddReport(report);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

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

namespace SNS.Application.Moderation.Commands.ReportComment;

public sealed record ReportCommentCommand(
    Guid CommentId,
    ViolationReason ViolationReason,
    string? AdditionalDetails
) : ICommand<Guid>;

internal sealed class ReportCommentCommandHandler : ICommandHandler<ReportCommentCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<ReportTicket> _ticketRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ReportCommentCommandHandler(
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

    public async Task<Result<Guid>> Handle(ReportCommentCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (!currentUserId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var commentExists = await _dbContext.Comments.AnyAsync(c => c.Id == request.CommentId && c.IsActive, cancellationToken);
        if (!commentExists)
        {
            return Result<Guid>.Failure(ResourceStatusCode.NotFound);
        }

        var ticket = await _ticketRepo.GetSingleByExpressionAsync(
            t => t.TargetId == request.CommentId && t.TargetType == ReportTargetType.Comment, 
            cancellationToken);

        if (ticket == null)
        {
            ticket = ReportTicket.Create(request.CommentId, ReportTargetType.Comment);
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

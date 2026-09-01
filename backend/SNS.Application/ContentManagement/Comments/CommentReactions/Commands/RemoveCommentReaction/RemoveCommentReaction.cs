using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.ContentManagement.Comments.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Comments.CommentReactions.Commands.RemoveCommentReaction;

public sealed record RemoveCommentReactionCommand(
    Guid CommentId
) : ICommand;

internal sealed class RemoveCommentReactionCommandHandler : ICommandHandler<RemoveCommentReactionCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<CommentReaction> _reactionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public RemoveCommentReactionCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<CommentReaction> reactionRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _reactionRepo = reactionRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(RemoveCommentReactionCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var comment = await _dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId, cancellationToken);

        if (comment == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var existingReaction = await _reactionRepo.GetSingleByExpressionAsync(
            r => r.CommentId == request.CommentId && r.ReactorId == profileId.Value, cancellationToken);

        if (existingReaction == null)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        _reactionRepo.Delete(existingReaction);
        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<CommentReactionRemovedIntegrationEvent>(
                new CommentReactionRemovedIntegrationEvent(comment.AuthorId, comment.Id, existingReaction.Id, profileId.Value, existingReaction.Type, DateTime.UtcNow)),
            cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

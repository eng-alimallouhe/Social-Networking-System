using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.ContentManagement.Comments.Events;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Comments.CommentReactions.Commands.AddOrChangeCommentReaction;

public sealed record AddOrChangeCommentReactionCommand(
    Guid CommentId,
    ReactionType ReactionType
) : ICommand;

internal sealed class AddOrChangeCommentReactionCommandHandler : ICommandHandler<AddOrChangeCommentReactionCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<CommentReaction> _reactionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public AddOrChangeCommentReactionCommandHandler(
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

    public async Task<Result> Handle(AddOrChangeCommentReactionCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var comment = await _dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == request.CommentId && c.IsActive, cancellationToken);

        if (comment == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var existingReaction = await _reactionRepo.GetSingleByExpressionAsync(
            r => r.CommentId == request.CommentId && r.ReactorId == profileId.Value, cancellationToken);

        if (existingReaction != null)
        {
            if (existingReaction.Type != request.ReactionType)
            {
                existingReaction.UpdateType(request.ReactionType);
                await _unitOfWork.CompleteAsync(cancellationToken);
            }
            return Result.Success(OperationStatusCode.Success);
        }

        var newReaction = CommentReaction.Create(request.CommentId, profileId.Value, request.ReactionType);
        _reactionRepo.Add(newReaction);
        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<CommentReactionAddedIntegrationEvent>(
                new CommentReactionAddedIntegrationEvent(comment.AuthorId, comment.Id, newReaction.Id, profileId.Value, request.ReactionType, DateTime.UtcNow)),
            cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

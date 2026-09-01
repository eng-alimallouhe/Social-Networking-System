using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.ContentManagement.Posts.Events;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.PostReactions.Commands.AddOrChangePostReaction;

public sealed record AddOrChangePostReactionCommand(
    Guid PostId,
    ReactionType ReactionType
) : ICommand;

internal sealed class AddOrChangePostReactionCommandHandler : ICommandHandler<AddOrChangePostReactionCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<PostReaction> _reactionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;

    public AddOrChangePostReactionCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<PostReaction> reactionRepo,
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

    public async Task<Result> Handle(AddOrChangePostReactionCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId && p.IsActive, cancellationToken);
        if (post == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var existingReaction = await _reactionRepo.GetSingleByExpressionAsync(
            r => r.PostId == request.PostId && r.ReactorId == profileId.Value, cancellationToken);

        if (existingReaction != null)
        {
            if (existingReaction.Type != request.ReactionType)
            {
                existingReaction.UpdateType(request.ReactionType);
                await _unitOfWork.CompleteAsync(cancellationToken);
            }
            return Result.Success(OperationStatusCode.Success);
        }

        var newReaction = PostReaction.Create(request.PostId, profileId.Value, request.ReactionType);
        _reactionRepo.Add(newReaction);
        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<PostReactionAddedIntegrationEvent>(
                new PostReactionAddedIntegrationEvent(post.AuthorId, post.Id, newReaction.Id, profileId.Value, request.ReactionType, DateTime.UtcNow)),
            cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

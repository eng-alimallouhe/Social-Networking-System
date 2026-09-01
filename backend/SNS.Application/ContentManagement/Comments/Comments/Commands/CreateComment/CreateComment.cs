using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.ContentManagement.Posts.PostMentions.Helpers;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.ContentManagement.Comments.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Profiles;

namespace SNS.Application.ContentManagement.Comments.Comments.Commands.CreateComment;

public sealed record CreateCommentCommand(
    Guid PostId,
    Guid? ParentCommentId,
    string Content,
    List<Guid>? MentionedProfileIds = null
) : ICommand;

internal sealed class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Comment> _commentRepo;
    private readonly IRepository<CommentMention> _commentMentionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IReputationPolicyService _reputationPolicyService;
    private readonly IMediator _mediator;

    public CreateCommentCommandHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Comment> commentRepo,
        IRepository<CommentMention> commentMentionRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IReputationPolicyService reputationPolicyService,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _commentRepo = commentRepo;
        _commentMentionRepo = commentMentionRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _reputationPolicyService = reputationPolicyService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var profile = await _dbContext.Profiles
            .FirstOrDefaultAsync(p => p.Id == profileId.Value && p.IsActive, cancellationToken);

        if (profile == null)
        {
            return Result.Failure(ProfileStatusCodes.NotFound);
        }

        var commentsCreatedToday = await _dbContext.Comments
            .CountAsync(c => c.AuthorId == profileId.Value && c.CreatedAt >= DateTime.UtcNow.Date, cancellationToken);

        if (!_reputationPolicyService.CanCreateComment(profile.Reputation, commentsCreatedToday))
        {
            return Result.Failure(ProfileStatusCodes.DailyCommentLimitReached);
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
        }

        var postExists = await _dbContext.Posts
            .AnyAsync(p => p.Id == request.PostId && p.IsActive, cancellationToken);

        if (!postExists)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        if (request.ParentCommentId.HasValue)
        {
            var parentExists = await _dbContext.Comments
                .AnyAsync(c => c.Id == request.ParentCommentId.Value && c.PostId == request.PostId && c.IsActive, cancellationToken);

            if (!parentExists)
            {
                return Result.Failure(ResourceStatusCode.NotFound);
            }
        }

        // Validate mentions
        var contentMentionIds = MentionParser.ExtractMentionedProfileIds(request.Content);
        if (request.MentionedProfileIds != null && request.MentionedProfileIds.Any())
        {
            if (!request.MentionedProfileIds.All(id => contentMentionIds.Contains(id)))
            {
                return Result.Failure(OperationStatusCode.InvalidInput);
            }
        }

        if (contentMentionIds.Any())
        {
            var activeProfilesCount = await _dbContext.Profiles
                .CountAsync(p => contentMentionIds.Contains(p.Id) && p.IsActive, cancellationToken);

            if (activeProfilesCount != contentMentionIds.Count)
            {
                return Result.Failure(ProfileStatusCodes.NotFound);
            }
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var comment = Comment.Create(
                authorId: profileId.Value,
                postId: request.PostId,
                parentCommentId: request.ParentCommentId,
                content: request.Content);

            _commentRepo.Add(comment);

            foreach (var mentionedId in contentMentionIds)
            {
                var mention = new CommentMention
                {
                    CommentId = comment.Id,
                    ProfileId = mentionedId
                };
                _commentMentionRepo.Add(mention);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _mediator.Publish(
                new DomainEventNotification<CommentCreatedIntegrationEvent>(
                    new CommentCreatedIntegrationEvent(profileId.Value, comment.Id, comment.PostId, DateTime.UtcNow)),
                cancellationToken);

            return Result.Success(OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

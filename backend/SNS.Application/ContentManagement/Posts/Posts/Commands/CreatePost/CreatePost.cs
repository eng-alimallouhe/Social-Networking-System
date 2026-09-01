using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.ContentManagement.Posts.PostMentions.Helpers;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.Contracts.Storage;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.ContentManagement.Posts.Enums;
using SNS.Domain.ContentManagement.Posts.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.ContentManagement;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Profiles;

namespace SNS.Application.ContentManagement.Posts.Posts.Commands.CreatePost;

/// <summary>
/// Represents a command to create a new post with title, content, optional community association, media files, and optional mentions.
/// </summary>
/// <param name="CommunityId">Optional unique identifier of the target community if publishing to a community.</param>
/// <param name="Title">The title of the post.</param>
/// <param name="Content">The textual content of the post.</param>
/// <param name="IsPenned">Indicates whether the post should be pinned.</param>
/// <param name="Files">List of attached media files (images or videos) to upload.</param>
/// <param name="MentionedProfileIds">Optional list of mentioned profile IDs to validate against content markers.</param>
public sealed record CreatePostCommand(
    Guid? CommunityId,
    string Title,
    string Content,
    bool IsPenned,
    List<UploadedFile> Files,
    List<Guid>? MentionedProfileIds = null
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="CreatePostCommand"/> to create and publish a post.
/// </summary>
internal sealed class CreatePostCommandHandler
    : ICommandHandler<CreatePostCommand>
{
    private readonly ISoftDeletableRepository<Post> _postRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;
    private readonly IReputationPolicyService _reputationPolicyService;
    private readonly IMediator _mediator;

    private record MediaTask(
        Task<string> UploadTask,
        string ObjectKey);

    public CreatePostCommandHandler(
        ISoftDeletableRepository<Post> postRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService,
        IReputationPolicyService reputationPolicyService,
        IMediator mediator)
    {
        _postRepo = postRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
        _reputationPolicyService = reputationPolicyService;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var profile = await _dbContext.Profiles
            .FirstOrDefaultAsync(p => p.Id == profileId.Value && p.IsActive, cancellationToken);

        if (profile == null)
        {
            return Result.Failure(ProfileStatusCodes.NotFound);
        }

        var postsCreatedToday = await _dbContext.Posts
            .CountAsync(p => p.AuthorId == profileId.Value && p.CreatedAt >= DateTime.UtcNow.Date, cancellationToken);

        if (!_reputationPolicyService.CanCreatePost(profile.Reputation, postsCreatedToday))
        {
            return Result.Failure(ProfileStatusCodes.DailyPostLimitReached);
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

        var mediaTasks = new List<MediaTask>();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var postType = request.CommunityId != null ?
                PostType.Community : PostType.Profile;

            var post = Post.Create(
                authorId: profileId.Value,
                title: request.Title,
                content: request.Content,
                communityId: request.CommunityId,
                isPinned: request.IsPenned,
                status: PostStatus.Draft,
                type: postType,
                engagementScore: 0);

            if (request.CommunityId != null)
            {
                var communityDetails = await _dbContext
                    .Communities
                    .Where(c => c.Id == request.CommunityId.Value && c.Memberships.Any(cm => cm.MemberId == profileId.Value))
                    .Select(c => new
                    {
                        AllowPostWithoutApproval = c.Settings.AllowPostWithoutApproval,
                        CurrentProfileRole = c.Memberships.FirstOrDefault(cm => cm.MemberId == profileId.Value)!.Role
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (communityDetails == null)
                {
                    return Result.Failure(OperationStatusCode.AccessDenied);
                }

                if (communityDetails.CurrentProfileRole == CommunityRole.Member && 
                    !communityDetails.AllowPostWithoutApproval)
                {
                    post.ChangeStatus(PostStatus.Pending);
                }
                else
                {
                    post.ChangeStatus(PostStatus.Accepted);
                }
            }

            // Add mentions
            foreach (var mentionedProfileId in contentMentionIds)
            {
                post.Mentions.Add(new PostMention
                {
                    PostId = post.Id,
                    ProfileId = mentionedProfileId
                });
            }

            for (int i = 0; i < request.Files.Count; i++)
            {
                var type = request.Files[i].FileType == FileType.Image ?
                        MediaType.Image : MediaType.Video;

                var mediaObjectKey = $"posts/{post.Id}/{type}s/{Guid.NewGuid()}.{request.Files[i].Extension}";
                
                post.Media.Add(PostMedia.Create(
                    postId: post.Id,
                    objectKey: mediaObjectKey,
                    mimeType: request.Files[i].ContentType,
                    type: type,
                    order: i + 1));

                mediaTasks.Add(new MediaTask(
                    UploadTask: _fileStorageService.UploadFileAsync(
                        request.Files[i].Stream,
                        request.Files[i].ContentType,
                        mediaObjectKey,
                        cancellationToken),
                    ObjectKey: mediaObjectKey));
            }
            await Task.WhenAll(mediaTasks.Select(mwt => mwt.UploadTask).ToList());

            post.AddDomainEvent(new PostCreatedEvent(
                PostId: post.Id,
                OccurredOn: DateTime.UtcNow));

            _postRepo.Add(post);

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _mediator.Publish(
                new DomainEventNotification<PostCreatedIntegrationEvent>(
                    new PostCreatedIntegrationEvent(profileId.Value, post.Id, DateTime.UtcNow)),
                cancellationToken);

            return Result.Success(PostStatusCodes.PostSentToClassification);
        }
        catch
        {
            var mediaToDeleteTask = new List<Task>();
            foreach (var task in mediaTasks)
            {
                if (task.UploadTask.IsCompletedSuccessfully)
                {
                    mediaToDeleteTask.Add(_fileStorageService.DeleteFileAsync(
                        objectKey: task.ObjectKey));
                }
            }

            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            await Task.WhenAll(mediaToDeleteTask);
            throw;
        }
    }
}

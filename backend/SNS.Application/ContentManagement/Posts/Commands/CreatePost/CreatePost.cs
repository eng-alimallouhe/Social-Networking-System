using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Contracts.Storage;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.ContentManagement.Posts.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.ContentManagement;
using Microsoft.EntityFrameworkCore;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Shared.Enums;
using SNS.Domain.ContentManagement.Posts.Events;

namespace SNS.Application.ContentManagement.Posts.Commands.CreatePost;

/// <summary>
/// Represents a command to create a new post with title, content, optional community association, and media files.
/// </summary>
/// <param name="CommunityId">Optional unique identifier of the target community if publishing to a community.</param>
/// <param name="Title">The title of the post.</param>
/// <param name="Content">The textual content of the post.</param>
/// <param name="IsPenned">Indicates whether the post should be pinned.</param>
/// <param name="Files">List of attached media files (images or videos) to upload.</param>
public sealed record CreatePostCommand(
    Guid? CommunityId,
    string Title,
    string Content,
    bool IsPenned,
    List<UploadedFile> Files
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="CreatePostCommand"/> to create and publish a post.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves author profile ID.
/// 2. Determines post type (Profile or Community) and verifies community membership and posting approval rules if target is a community.
/// 3. Creates the <see cref="Post"/> entity and attaches media records.
/// 4. Uploads media files in parallel to storage via <see cref="IFileStorageService"/>.
/// 5. Raises a <see cref="PostCreatedEvent"/> domain event.
/// 6. Persists post entity inside a database transaction, rolling back uploaded media files if storage or database commit fails.
/// Side effects include storage file uploads, entity persistence, domain event publishing, and transaction commit.
/// </remarks>
internal sealed class CreatePostCoommandHandler
    : ICommandHandler<CreatePostCommand>
{
    private readonly ISoftDeletableRepository<Post> _postRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    private record MediaTask(
        Task<string> UploadTask,
        string ObjectKey);

    public CreatePostCoommandHandler(
        ISoftDeletableRepository<Post> postRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _postRepo = postRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
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


            for (int i = 0; i <= request.Files.Count; i++)
            {
                var type = request.Files[i].FileType == FileType.Image ?
                        MediaType.Image : MediaType.Video;

                var mediaObjectKey = $"posts/{post.Id}/{type}s/{Guid.NewGuid()}.{request.Files[i].Extension}";
                
                post.Media.Add(PostMedia.Create(
                    postId: post.Id,
                    objectKey: mediaObjectKey,
                    mimeType: request.Files[i].ContentType,
                    type: type,
                    order: i+1));

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
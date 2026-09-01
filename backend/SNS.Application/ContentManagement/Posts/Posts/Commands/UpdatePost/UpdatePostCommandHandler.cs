using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.ContentManagement.Posts.PostMentions.Helpers;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.Contracts.Storage;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.ContentManagement.Posts.Events;
using SNS.Domain.ContentManagement.Posts.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Profiles;

namespace SNS.Application.ContentManagement.Posts.Posts.Commands.UpdatePost;

/// <summary>
/// Represents a command to update an existing post's title, content, media attachments, tags, and mentions.
/// </summary>
/// <param name="PostId">The unique identifier of the post being updated.</param>
/// <param name="Title">The updated title of the post.</param>
/// <param name="Content">The updated textual content of the post.</param>
/// <param name="DeletedMediaIds">List of media attachment identifiers to delete.</param>
/// <param name="NewMedia">List of new uploaded media files to attach to the post.</param>
/// <param name="DeletedTagIds">List of tag identifiers to remove from the post.</param>
/// <param name="NewTagIds">List of new tag identifiers to associate with the post.</param>
/// <param name="MentionedProfileIds">Optional list of mentioned profile IDs to validate against content markers.</param>
public sealed record UpdatePostCommand(
    Guid PostId,
    string Title,
    string Content,
    List<Guid> DeletedMediaIds,
    List<UploadedFile> NewMedia,
    List<Guid> DeletedTagIds,
    List<Guid> NewTagIds,
    List<Guid>? MentionedProfileIds = null
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdatePostCommand"/> to modify post details, media, and mentions.
/// </summary>
internal class UpdatePostCommandHandler
    : ICommandHandler<UpdatePostCommand>
{
    private readonly ISoftDeletableRepository<Post> _postRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAppLogger<UpdatePostCommandHandler> _logger;

    private record MediaTask(
        Task<string> UploadTask,
        string ObjectKey);

    public UpdatePostCommandHandler(
        ISoftDeletableRepository<Post> postRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService,
        IAppLogger<UpdatePostCommandHandler> logger)
    {
        _postRepo = postRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var spec = new PostToUpdateSpecification(request.PostId);
        var post = await _postRepo.GetSingleAsync(spec, cancellationToken);

        if (post == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        if (post.AuthorId != profileId.Value)
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
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

        var requiresReclassification =
            post.Content != request.Content ||
            post.Title != request.Title ||
            request.NewMedia.Any() ||
            request.DeletedMediaIds.Any();

        post.UpdateInfo(request.Title, request.Content);

        var oldObjectKeysToDelete = new List<string>();
        
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (request.DeletedMediaIds.Any())
            {
                var deletedIds = request.DeletedMediaIds.ToHashSet();

                var deletedMedia = post.Media
                    .Where(m => deletedIds.Contains(m.Id))
                    .ToList();

                oldObjectKeysToDelete.AddRange(
                    deletedMedia.Select(m => m.ObjectKey));

                post.Media.RemoveAll(
                    m => deletedIds.Contains(m.Id));
            }

            int newOrder = 0;
            foreach (var media in post.Media)
            {
                newOrder++;
                media.SetOrder(newOrder);
            }

            foreach (var media in request.NewMedia)
            {
                var folder = media.FileType == FileType.Image
                    ? "images"
                    : "videos";

                var objectKey =
                    $"posts/{post.Id}/{folder}/{Guid.NewGuid()}.{media.Extension}";

                mediaTasks.Add(new MediaTask(
                    _fileStorageService.UploadFileAsync(
                        media.Stream,
                        media.ContentType,
                        objectKey,
                        cancellationToken),
                    objectKey));

                post.Media.Add(PostMedia.Create(
                    postId: post.Id,
                    objectKey: objectKey,
                    mimeType: media.ContentType,
                    order: ++newOrder,
                    type: media.FileType == FileType.Image
                        ? MediaType.Image
                        : MediaType.Video));
            }

            await Task.WhenAll(mediaTasks.Select(mt => mt.UploadTask));

            if (request.DeletedTagIds.Any())
            {
                var deletedIds = request.DeletedTagIds.ToHashSet();

                post.PostTags.RemoveAll(
                   m => deletedIds.Contains(m.Id));
            }

            if (request.NewTagIds.Any())
            {
                var tags = await _dbContext
                    .Tags
                    .Where(t => request.NewTagIds.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToListAsync(cancellationToken);

                post.PostTags.AddRange(tags.Select(t => PostTag.Create(postId: post.Id, tagId: t)));
            }

            // Synchronize mentions
            var existingMentionIds = post.Mentions.Select(m => m.ProfileId).ToHashSet();

            // 1. Remove obsolete mentions
            var obsoleteMentions = post.Mentions.Where(m => !contentMentionIds.Contains(m.ProfileId)).ToList();
            foreach (var obsolete in obsoleteMentions)
            {
                post.Mentions.Remove(obsolete);
            }

            // 2. Add newly introduced mentions
            foreach (var newMentionId in contentMentionIds.Where(id => !existingMentionIds.Contains(id)))
            {
                post.Mentions.Add(new PostMention
                {
                    PostId = post.Id,
                    ProfileId = newMentionId
                });
            }

            post.AddDomainEvent(new PostUpdatedEvent(PostId: post.Id, RequiresReclassification: requiresReclassification, OccurredOn: DateTime.UtcNow));
            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch 
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            var objectsToDelete = new List<string>();

            foreach (var task in mediaTasks)
            {
                if (task.UploadTask.IsCompletedSuccessfully)
                {
                    objectsToDelete.Add(task.ObjectKey);
                }
            }

            await Task.WhenAll(
                objectsToDelete.Select(x =>
                    _fileStorageService.DeleteFileAsync(
                        x,
                        cancellationToken)));

            throw;
        }

        foreach (var objectKey in oldObjectKeysToDelete)
        {
            try
            {
                await _fileStorageService.DeleteFileAsync(
                    objectKey,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    "Failed to delete media {ObjectKey}",
                    ex,
                    objectKey);
            }
        }

        return Result.Success(OperationStatusCode.Success);
    }
}

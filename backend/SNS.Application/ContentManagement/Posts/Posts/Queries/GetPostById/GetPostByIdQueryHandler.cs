using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Comments.Comments.Contracts;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Posts.PostMentions.Contracts;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Posts.Posts.Queries.GetPostById;

internal sealed class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, PostDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetPostByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<PostDetailsDto>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        var rawPost = await _dbContext.Posts
            .AsNoTracking()
            .Where(p => p.Id == request.PostId && p.IsActive)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Content,
                p.IsPinned,
                p.Type,
                p.Status,
                p.EngagementScore,
                SaveCount = p.SavedPosts.Count(),
                p.CreatedAt,
                p.UpdatedAt,

                AuthorId = p.Author.Id,
                AuthorFullName = p.Author.FullName,
                AuthorSpecialization = p.Author.Specialization,
                AuthorProfilePictureKey = p.Author.ProfilePictureObjectKey,

                p.CommunityId,
                CommunityType = p.Community != null ? p.Community.Type : (SNS.Domain.ContentManagement.Communities.Enums.CommunityType?)null,
                CommunityName = p.Community != null ? p.Community.Name : null,
                CommunityLogoKey = p.Community != null ? p.Community.LogoObjectKey : null,

                Media = p.Media.OrderBy(m => m.Order).Select(m => new { m.ObjectKey, m.Order, m.Type }).ToList(),
                TotalComments = p.Comments.Count(c => c.IsActive),
                ReactionCount = p.Reactions.Count(),

                FirstComments = p.Comments
                    .Where(c => c.IsActive && c.ParentCommentId == null)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(10)
                    .Select(c => new
                    {
                        c.Id,
                        c.PostId,
                        c.ParentCommentId,
                        c.Content,
                        c.CreatedAt,
                        c.UpdatedAt,
                        AuthorId = c.AuthorId,
                        AuthorFullName = c.Author.FullName,
                        AuthorSpecialization = c.Author.Specialization,
                        AuthorProfilePictureKey = c.Author.ProfilePictureObjectKey,
                        ReactionsCount = c.Reactions.Count(),
                        RepliesCount = c.Replies.Count(r => r.IsActive),
                        CurrentUserReaction = profileId != null ? c.Reactions.Where(r => r.ReactorId == profileId).Select(r => (ReactionType?)r.Type).FirstOrDefault() : null
                    })
                    .ToList(),

                Tags = p.PostTags.Select(t => t.Tag.Name).ToList(),

                Mentions = p.Mentions
                    .Where(m => m.Profile.IsActive)
                    .Select(m => new
                    {
                        m.ProfileId,
                        DisplayName = m.Profile.FullName,
                        ProfilePictureKey = m.Profile.ProfilePictureObjectKey
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (rawPost == null)
        {
            return Result<PostDetailsDto>.Failure(ResourceStatusCode.NotFound);
        }

        // Collect all distinct storage keys
        var distinctKeys = new List<string?>
        {
            rawPost.AuthorProfilePictureKey,
            rawPost.CommunityLogoKey
        };
        distinctKeys.AddRange(rawPost.Media.Select(m => (string?)m.ObjectKey));
        distinctKeys.AddRange(rawPost.FirstComments.Select(c => c.AuthorProfilePictureKey));
        distinctKeys.AddRange(rawPost.Mentions.Select(m => m.ProfilePictureKey));

        var validKeys = distinctKeys.Where(k => !string.IsNullOrWhiteSpace(k)).Distinct().ToList();

        var urlTasks = validKeys.Select(async k => new
        {
            Key = k!,
            Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
        });
        var resolvedUrls = await Task.WhenAll(urlTasks);
        var urlMap = resolvedUrls.ToDictionary(r => r.Key, r => r.Url);

        var authorSnapshot = new ProfileSnapshotDto(
            rawPost.AuthorId,
            rawPost.AuthorFullName,
            rawPost.AuthorSpecialization,
            rawPost.AuthorProfilePictureKey != null && urlMap.TryGetValue(rawPost.AuthorProfilePictureKey, out var authorPicUrl) ? authorPicUrl : null
        );

        var communitySnapshot = rawPost.CommunityId.HasValue && rawPost.CommunityType.HasValue && rawPost.CommunityName != null
            ? new CommunitySnapshotDto(
                rawPost.CommunityId.Value,
                rawPost.CommunityName,
                rawPost.CommunityType.Value,
                rawPost.CommunityLogoKey != null && urlMap.TryGetValue(rawPost.CommunityLogoKey, out var commLogoUrl) ? commLogoUrl : null)
            : null;

        var mediaList = rawPost.Media.Select(m => new PostMediaDto(
            Url: urlMap.TryGetValue(m.ObjectKey, out var mediaUrl) ? mediaUrl : m.ObjectKey,
            Order: m.Order,
            Type: m.Type
        )).ToList();

        var commentsList = rawPost.FirstComments.Select(c => new CommentSummaryDto(
            c.Id,
            c.PostId,
            c.ParentCommentId,
            c.Content,
            c.CreatedAt,
            c.UpdatedAt,
            new ProfileSnapshotDto(
                c.AuthorId,
                c.AuthorFullName,
                c.AuthorSpecialization,
                c.AuthorProfilePictureKey != null && urlMap.TryGetValue(c.AuthorProfilePictureKey, out var cAuthorPicUrl) ? cAuthorPicUrl : null
            ),
            c.ReactionsCount,
            c.RepliesCount,
            c.CurrentUserReaction
        )).ToList();

        var pagedComments = new Paged<CommentSummaryDto>(
            commentsList,
            rawPost.TotalComments,
            pageSize: 10,
            currentPage: 1
        );

        var mentionsList = rawPost.Mentions.Select(m => new PostMentionDto(
            m.ProfileId,
            m.DisplayName,
            m.ProfilePictureKey != null && urlMap.TryGetValue(m.ProfilePictureKey, out var mPicUrl) ? mPicUrl : null
        )).ToList();

        var postDetails = new PostDetailsDto(
            rawPost.Id,
            rawPost.Title,
            rawPost.Content,
            rawPost.IsPinned,
            rawPost.Type,
            rawPost.Status,
            rawPost.EngagementScore,
            rawPost.SaveCount,
            rawPost.CreatedAt,
            rawPost.UpdatedAt,
            authorSnapshot,
            communitySnapshot,
            mediaList,
            pagedComments,
            rawPost.Tags,
            rawPost.ReactionCount,
            mentionsList
        );

        return Result<PostDetailsDto>.Success(postDetails, OperationStatusCode.Success);
    }
}

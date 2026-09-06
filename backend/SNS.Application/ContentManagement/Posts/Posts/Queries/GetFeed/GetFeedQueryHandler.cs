using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Posts.PostMentions.Contracts;
using SNS.Application.ContentManagement.Posts.Posts.Abstractions;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Search.ContentManagement.Posts.Queries;
using SNS.Application.Shared.Abstractions.BackgroundJobs;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.Posts.Queries.GetFeed;

/// <summary>
/// Handles <see cref="GetFeedQuery"/> to compile and return a personalized post feed for the authenticated user.
/// </summary>
internal sealed class GetFeedQueryHandler
    : IQueryHandler<GetFeedQuery, List<PostOverviewDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPostCacheService _postCacheService;
    private readonly IFeedFallbackService _feedFallbackService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IJobSchedulerService _jobSchedulerService;

    public GetFeedQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IPostCacheService postCacheService,
        IFeedFallbackService feedFallbackService,
        IFileStorageService fileStorageService,
        IJobSchedulerService jobSchedulerService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _postCacheService = postCacheService;
        _feedFallbackService = feedFallbackService;
        _fileStorageService = fileStorageService;
        _jobSchedulerService = jobSchedulerService;
    }

    public async Task<Result<List<PostOverviewDto>>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<List<PostOverviewDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var sortedFeed = await _postCacheService.GetProfileFeed(profileId.Value, request.CurrentPage, request.PageSize, cancellationToken);
        if (sortedFeed.Any())
        {
            var postsIds = sortedFeed.Select(fi => fi.PostId).ToList();
            var rawPosts = await _dbContext.Posts
                .AsNoTracking()
                .Where(p => postsIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    AuthorId = p.Author.Id,
                    AuthorFullName = p.Author.FullName,
                    AuthorSpecialization = p.Author.Specialization,
                    AuthorProfilePictureKey = p.Author.ProfilePictureObjectKey,

                    p.CommunityId,
                    CommunityType = p.Community != null ? p.Community.Type : (SNS.Domain.ContentManagement.Communities.Enums.CommunityType?)null,
                    CommunityName = p.Community != null ? p.Community.Name : null,
                    CommunityLogoKey = p.Community != null ? p.Community.LogoObjectKey : null,

                    p.Title,
                    p.Content,
                    p.CreatedAt,
                    p.UpdatedAt,
                    p.LastInteractedAt,

                    Media = p.Media
                        .OrderBy(m => m.Order)
                        .Select(m => new { m.ObjectKey, m.Order, m.Type })
                        .ToList(),

                    Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                    CommentsCount = p.Comments.Count(c => c.IsActive),
                    ReactionsCount = p.Reactions.Count(),
                    ViewsCount = p.Views.Count(),
                    SavesCount = p.SavedPosts.Count(),
                    CurrentUserReaction = p.Reactions.Where(r => r.ReactorId == profileId).Select(r => (ReactionType?)r.Type).FirstOrDefault(),

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
                .ToListAsync(cancellationToken);

            var distinctKeys = rawPosts
                .Select(p => p.AuthorProfilePictureKey)
                .Concat(rawPosts.Select(p => p.CommunityLogoKey))
                .Concat(rawPosts.SelectMany(p => p.Media.Select(m => m.ObjectKey)))
                .Concat(rawPosts.SelectMany(p => p.Mentions.Select(m => m.ProfilePictureKey)))
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .Distinct()
                .ToList();

            var urlTasks = distinctKeys.Select(async k => new
            {
                Key = k!,
                Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
            });
            var resolvedUrls = await Task.WhenAll(urlTasks);
            var urlMap = resolvedUrls.ToDictionary(r => r.Key, r => r.Url);

            var postDtos = rawPosts.Select(p => new PostOverviewDto(
                Id: p.Id,
                Author: new ProfileSnapshotDto(
                    p.AuthorId,
                    p.AuthorFullName,
                    p.AuthorSpecialization,
                    p.AuthorProfilePictureKey != null && urlMap.TryGetValue(p.AuthorProfilePictureKey, out var authorPicUrl) ? authorPicUrl : null
                ),
                Community: p.CommunityId.HasValue && p.CommunityType.HasValue && p.CommunityName != null
                    ? new CommunitySnapshotDto(
                        p.CommunityId.Value,
                        p.CommunityName,
                        p.CommunityType.Value,
                        p.CommunityLogoKey != null && urlMap.TryGetValue(p.CommunityLogoKey, out var commLogoUrl) ? commLogoUrl : null)
                    : null,
                Title: p.Title,
                Content: p.Content,
                CreatedAt: p.CreatedAt,
                UpdatedAt: p.UpdatedAt,
                LastInteractedAt: p.LastInteractedAt,
                Media: p.Media.Select(m => new PostMediaDto(
                    Url: urlMap.TryGetValue(m.ObjectKey, out var mediaUrl) ? mediaUrl : m.ObjectKey,
                    Order: m.Order,
                    Type: m.Type
                )).ToList(),
                Tags: p.Tags,
                CommentsCount: p.CommentsCount,
                ReactionsCount: p.ReactionsCount,
                ViewsCount: p.ViewsCount,
                SavesCount: p.SavesCount,
                CurrentUserReaction: p.CurrentUserReaction,
                Mentions: p.Mentions.Select(m => new PostMentionDto(
                    m.ProfileId,
                    m.DisplayName,
                    m.ProfilePictureKey != null && urlMap.TryGetValue(m.ProfilePictureKey, out var mentionPicUrl) ? mentionPicUrl : null
                )).ToList()
            )).ToList();

            var orderedPosts = postsIds
                .Join(
                    postDtos,
                    id => id,
                    p => p.Id,
                    (_, post) => post)
                .ToList();

            return Result<List<PostOverviewDto>>.Success(orderedPosts, ResourceStatusCode.Found);
        }

        if (request.CurrentPage >= 1)
        {
            var followedProfileIds = await _dbContext.Follows
                .Where(f => f.FollowerId == profileId.Value)
                .Select(f => f.FollowingId)
                .ToListAsync(cancellationToken);

            var communityIds = await _dbContext.CommunityMemberships
                .Where(cm => cm.MemberId == profileId.Value)
                .Select(cm => cm.CommunityId)
                .ToListAsync(cancellationToken);

            var feedParams = new FeedRequestParameter(
                ProfileId: profileId.Value,
                Skills: new List<string>(),
                ExcludedPostsIds: new List<Guid>(),
                ExcludedProfilesIds: new List<Guid>(),
                CommunitiesIds: communityIds,
                FollowedProfilesIds: followedProfileIds,
                StartDate: DateTime.UtcNow.AddYears(-7),
                Topics: new List<ProfileTopicSnapshot>(),
                Tags: new List<ProfileTagSnapshot>(),
                FeedSize: 100
            );

            var initialFeed = await _feedFallbackService.GetFallbackFeedAsync(new FeedRequestParameter(
                ProfileId: profileId.Value,
                Skills: new List<string>(),
                ExcludedPostsIds: new List<Guid>(),
                ExcludedProfilesIds: new List<Guid>(),
                CommunitiesIds: communityIds,
                FollowedProfilesIds: followedProfileIds,
                StartDate: DateTime.UtcNow.AddYears(-7),
                Topics: new List<ProfileTopicSnapshot>(),
                Tags: new List<ProfileTagSnapshot>(),
                FeedSize: request.PageSize
            ), request.PageSize, cancellationToken);

            if (initialFeed.Any())
            {
                await _postCacheService.SetProfileFeedAsync(
                    profileId.Value, 
                    initialFeed.Select(f => new FeedItemModel(f.Id, 0.0)).ToList(), 
                    cancellationToken);
            }

            _jobSchedulerService.Enqueue<IFeedBackgroundService>(
                service => service.ComputeAndCacheUserFeedAsync(
                    profileId.Value, 
                    feedParams));

            return Result<List<PostOverviewDto>>.Success(initialFeed, OperationStatusCode.Success);
        }

        return Result<List<PostOverviewDto>>.Success(new List<PostOverviewDto>(), ResourceStatusCode.Found);
    }
}

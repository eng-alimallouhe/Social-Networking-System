using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Posts.Abstractions;
using SNS.Application.ContentManagement.Posts.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Queries;
using SNS.Application.Shared.Abstractions.BackgroundJobs;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.Queries.GetFeed;

/// <summary>
/// Represents a query to retrieve a personalized feed of posts for the authenticated user.
/// </summary>
/// <param name="CurrentPage">The current page index for feed pagination (1-based).</param>
/// <param name="PageSize">The maximum number of posts to retrieve per page.</param>
public sealed record GetFeedQuery(
    int CurrentPage,
    int PageSize
) : IQuery<List<PostOverviewDto>>;

/// <summary>
/// Handles the execution of <see cref="GetFeedQuery"/> to retrieve a personalized post feed.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Attempts to fetch cached feed post IDs for the user profile from <see cref="IPostCacheService"/>.
/// 2. If cached feed exists, projects detailed post overviews (author, community, media, tags, engagement counters) from database ordered by cached ranking.
/// 3. If cache is empty, gathers profile feed parameters (skills, topics, tags, followings, community memberships, block list).
/// 4. Enqueues a background job via <see cref="IJobSchedulerService"/> to asynchronously compute and cache user feed.
/// 5. Retrieves and returns an instant fallback feed via <see cref="IFeedFallbackService"/>.
/// </remarks>
internal class GetFeedQueryHandler
    : IQueryHandler<GetFeedQuery, List<PostOverviewDto>>
{
    private readonly IPostSearchService _postSearchService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPostCacheService _postCacheService;
    private readonly DateTime _startDate = DateTime.UtcNow.AddYears(-4);
    private readonly IJobSchedulerService _backgroundJobService;
    private readonly IFeedBackgroundService _feedBackgroundService;
    private readonly IFeedFallbackService _feedFallbackService;

    public GetFeedQueryHandler(
        IPostSearchService postSearchService,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IPostCacheService postCacheService,
        IJobSchedulerService backgroundJobService,
        IFeedFallbackService feedFallbackService,
        IFeedBackgroundService feedBackgroundService)
    {
        _postSearchService = postSearchService;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _postCacheService = postCacheService;
        _backgroundJobService = backgroundJobService;
        _feedFallbackService = feedFallbackService;
        _feedBackgroundService = feedBackgroundService;
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
            var posts = await _dbContext.Posts
            .Where(p =>
                postsIds.Contains(p.Id)
            )
            .Select(p => new PostOverviewDto(
                Id: p.Id,
                AuthorId: p.AuthorId,
                AuthorName: p.Author.FullName,
                AuthorSpecialization: p.Author.Specialization,
                AuthorProfilePictureUrl: p.Author.ProfilePictureObjectKey,

                CommunityId: p.CommunityId,
                CommunityType: p.Community != null ? p.Community.Type : null,
                CommunityName: p.Community != null ? p.Community.Name : null,
                CommunityLogoUrl: p.Community != null ? p.Community.LogoObjectKey : null,

                Title: p.Title,
                Content: p.Content,

                CreatedAt: p.CreatedAt,
                UpdatedAt: p.UpdatedAt,
                LastInteractedAt: p.LastInteractedAt,

                FirstMediaUrl: p.Media
                    .OrderBy(m => m.Order)
                    .Select(m => m.ObjectKey)
                    .FirstOrDefault() ?? string.Empty,

                MediaCount: p.Media.Count(),

                Tags: p.PostTags
                    .Select(pt => pt.Tag.Name)
                    .ToList(),

                CommentsCount: p.Comments.Count(),

                ReactionsCount: p.Reactions.Count(),

                ViewsCount: p.Views.Count(),

                SavesCount: p.SavedPosts.Count()
            ))
            .ToListAsync(cancellationToken);

            var orderedPosts = postsIds
                .Join(
                    posts,
                    id => id,
                    p => p.Id,
                    (_, post) => post)
                .ToList();

            return Result<List<PostOverviewDto>>.Success(orderedPosts, ResourceStatusCode.Found);
        }

        var feedParams = await _dbContext
                .Profiles
                .Where(p => p.Id == profileId)
                .Select(p => new FeedRequestParameter(
                    ProfileId: p.Id,
                    Skills: p.ProfileSkills.Select(p => p.Skill.Name).ToList(),
                    ExcludedPostsIds: _dbContext.PostViews.Where(pv => pv.ViewerId == profileId && pv.IsActive).Select(pv => pv.PostId).ToList(),
                    ExcludedProfilesIds: _dbContext.Blocks
                        .Where(b => b.BlockerId == profileId || b.BlockedId == profileId)
                        .Select(b => b.BlockerId == profileId ? b.BlockedId : b.BlockerId)
                        .ToList(),
                    CommunitiesIds: p.Memberships.Select(cm => cm.CommunityId).ToList(),
                    FollowedProfilesIds: p.Followings.Select(f => f.FollowingId).ToList(),
                    StartDate: DateTime.UtcNow.AddYears(-4),
                    Topics: p.ProfileTopics.Select(pt => new ProfileTopicSnapshot(pt.TopicId, pt.Score)).ToList(),
                    Tags: p.ProfileTags.Select(pt => new ProfileTagSnapshot(pt.TagId, pt.Score)).ToList(),
                    FeedSize: 400
                ))
                .FirstOrDefaultAsync();

        if (feedParams == null)
        {
            return Result<List<PostOverviewDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        _backgroundJobService.Enqueue<IFeedBackgroundService>(x => x.ComputeAndCacheUserFeedAsync(profileId.Value, feedParams));

        var initialFeed = await _feedFallbackService.GetFallbackFeedAsync(feedParams, cancellationToken: cancellationToken);

        return Result<List<PostOverviewDto>>.Success(initialFeed, OperationStatusCode.Success);
    }
}
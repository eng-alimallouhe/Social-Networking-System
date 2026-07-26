using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Posts.Abstractions;
using SNS.Application.ContentManagement.Posts.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Queries;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.Queries.GetFeed;

public sealed record GetFeedQuery(
) : IQuery<List<PostOverviewDto>>;

internal class GetFeedQueryHandler
    : IQueryHandler<GetFeedQuery, List<PostOverviewDto>>
{
    private readonly IPostSearchService _postSearchService;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPostCacheService _postCacheService;
    private readonly DateTime _startDate = DateTime.UtcNow.AddYears(-4);

    public GetFeedQueryHandler(
        IPostSearchService postSearchService,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IPostCacheService postCacheService)
    {
        _postSearchService = postSearchService;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _postCacheService = postCacheService;
    }

    public async Task<Result<List<PostOverviewDto>>> Handle(GetFeedQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<List<PostOverviewDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var feedModels = await _postCacheService.GetProfileFeed(profileId.Value);

        if (feedModels.Count != 0)
        {
            var feedIds = feedModels.
                Take(15)
                .Select(fi => fi.PostId)
                .ToList();

            var clculatedFeed = await _dbContext
            .Posts
            .Where(p => feedIds.Contains(p.Id))
            .Select(p => new PostOverviewDto(
                Id: p.Id,
                AuthorId: p.AuthorId,
                AuthorName: p.Author.FullName,
                AuthorSpecialization: p.Author.Specialization,
                AuthorProfilePictureUrl: p.Author.ProfilePictureObjectKey,
                CommunityId: p.CommunityId,
                CommunityType: p.Community == null ? null : p.Community.Type,
                CommunityName: p.Community == null ? null : p.Community.Name,
                CommunityLogoUrl: p.Community == null
                    ? null
                    : p.Community.LogoObjectKey,
                Title: p.Title,
                Content: p.Content,
                CreatedAt: p.CreatedAt,
                UpdatedAt: p.UpdatedAt,
                LastInteractedAt: p.LastInteractedAt,
                FirstMediaUrl: p.Media
                    .OrderBy(m => m.Order)
                    .Select(m => m.ObjectKey)
                    .FirstOrDefault()!,
                MediaCount: p.Media.Count(),
                Tags: p.PostTags
                    .OrderBy(pt => pt.Tag.Name)
                    .Select(pt => pt.Tag.Name)
                    .ToList(),
                CommentsCount: p.Comments.Count(),
                ReactionsCount: p.Reactions.Count(),
                ViewsCount: p.Views.Count(),
                SavesCount: p.SavedPosts.Count()
            ))
            .ToListAsync(cancellationToken);

            return Result<List<PostOverviewDto>>.Success(clculatedFeed, ResourceStatusCode.Found);
        }

        var feedParams = await _dbContext
            .Profiles
            .Where(p => p.Id == profileId.Value)
            .Select(p => new FeedRequestParameter(
                ProfileId: p.Id,
                Skills: p.ProfileSkills.Select(p => p.Skill.Name).ToList(),
                ExcludedPostsIds: _dbContext.PostViews.Where(pv => pv.ViewerId == profileId && pv.IsActive).Select(pv => pv.PostId).ToList(),
                ExcludedProfilesIds: _dbContext.Blocks
                    .Where(b =>
                        b.BlockerId == profileId.Value ||
                        b.BlockedId == profileId.Value)
                    .Select(b =>
                        b.BlockerId == profileId.Value
                            ? b.BlockedId
                            : b.BlockerId)
                    .ToList(),
                CommunitiesIds: p.Memberships.Select(cm => cm.CommunityId).ToList(),
                FollowedProfilesIds: p.Followings.Select(f => f.FollowingId).ToList(),
                StartDate: DateTime.UtcNow.AddYears(-4),
                Topics: p.ProfileTopics.Select(pt => new ProfileTopicSnapshot(pt.Topic.Name, pt.Score)).ToList(),
                Tags: p.ProfileTags.Select(pt => new ProfileTagSnapshot(pt.Tag.Name, pt.Score)).ToList(),
                FeedSize: 200
                ))
            .FirstOrDefaultAsync(cancellationToken);

        if (feedParams == null)
        {
            return Result<List<PostOverviewDto>>.Failure(ResourceStatusCode.NotFound);
        }

        Console.WriteLine($"Start Date: {feedParams.StartDate}");

        var feed = await _postSearchService.GetFeedPostsAsync(
            feedParams,
            cancellationToken);

        var firstPosts = feed
            .Take(20)
            .ToList();

        var remainingPosts = feed 
            .Skip(20)
            .Select(x => new FeedItemModel(
                x.PostId,
                x.Score))
            .ToList();

        await _postCacheService.AddPostToProfileFeed(
            profileId.Value,
            remainingPosts,
            cancellationToken);

        var firstIds = firstPosts
            .Select(x => x.PostId)
            .ToList();

        var posts = await _dbContext
            .Posts
            .Where(p => firstIds.Contains(p.Id))
            .Select(p => new PostOverviewDto(
                Id: p.Id,
                AuthorId: p.AuthorId,
                AuthorName: p.Author.FullName,
                AuthorSpecialization: p.Author.Specialization,
                AuthorProfilePictureUrl: p.Author.ProfilePictureObjectKey,
                CommunityId: p.CommunityId,
                CommunityType: p.Community == null ? null : p.Community.Type,
                CommunityName: p.Community == null ? null : p.Community.Name,
                CommunityLogoUrl: p.Community == null
                    ? null
                    : p.Community.LogoObjectKey,
                Title: p.Title,
                Content: p.Content,
                CreatedAt: p.CreatedAt,
                UpdatedAt: p.UpdatedAt,
                LastInteractedAt: p.LastInteractedAt,
                FirstMediaUrl: p.Media
                    .OrderBy(m => m.Order)
                    .Select(m => m.ObjectKey)
                    .FirstOrDefault()!,
                MediaCount: p.Media.Count(),
                Tags: p.PostTags
                    .OrderBy(pt => pt.Tag.Name)
                    .Select(pt => pt.Tag.Name)
                    .ToList(),
                CommentsCount: p.Comments.Count(),
                ReactionsCount: p.Reactions.Count(),
                ViewsCount: p.Views.Count(),
                SavesCount: p.SavedPosts.Count()
            ))
            .ToListAsync(cancellationToken);

        return Result<List<PostOverviewDto>>.Success(posts, ResourceStatusCode.Found);
    }
}

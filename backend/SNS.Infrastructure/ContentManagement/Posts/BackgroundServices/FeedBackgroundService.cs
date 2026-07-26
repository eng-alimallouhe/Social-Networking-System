using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Queries;
using SNS.Application.Shared.Abstractions.Data;

namespace SNS.Infrastructure.ContentManagement.Posts.BackgroundServices;


public class FeedBackgroundService : IFeedBackgroundService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPostCacheService _postCacheService;


    public FeedBackgroundService(
        IApplicationDbContext dbContext,
        IPostCacheService postCacheService)
    {
        _dbContext = dbContext;
        _postCacheService = postCacheService;
    }

    public async Task ComputeAndCacheUserFeedAsync(Guid profileId)
    {
        var feedParams = await _dbContext
            .Profiles
            .Where(p => p.Id == profileId)
            .Select(p => new FeedRequestParameter(
                ProfileId: p.Id,
                Skills: p.ProfileSkills.Select(p => p.Skill.Name).ToList(),
                ExcludedPostsIds: _dbContext.PostViews.Where(pv => pv.ViewerId == profileId && pv.IsActive).Select(pv => pv.PostId).ToList(),
                ExcludedProfilesIds: _dbContext.Blocks
                    .Where(b =>
                        b.BlockerId == profileId ||
                        b.BlockedId == profileId)
                    .Select(b =>
                        b.BlockerId == profileId
                            ? b.BlockedId
                            : b.BlockerId)
                    .ToList(),
                CommunitiesIds: p.Memberships.Select(cm => cm.CommunityId).ToList(),
                FollowedProfilesIds: p.Followings.Select(f => f.FollowingId).ToList(),
                StartDate: DateTime.UtcNow.AddYears(-4),
                Topics: p.ProfileTopics.Select(pt => new ProfileTopicSnapshot(pt.Topic.Name, pt.Score)).ToList(),
                Tags: p.ProfileTags.Select(pt => new ProfileTagSnapshot(pt.Tag.Name, pt.Score)).ToList(),
                ))
            .FirstOrDefaultAsync();

        var firstBatchOfPosts = await _dbContext
            .Posts
            .Where(p => )
            .Take(500)
            .Select()
            .ToListAsync();

        var firstBatch = await _dbContext
            .Posts;
    }
}

using SNS.Application.Abstractions.Caching;
using SNS.Application.ContentManagement.Posts.Posts.Abstractions;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Posts.Posts.Services;

public sealed class PostCacheService
    : IPostCacheService
{
    private readonly ICacheService _cacheService;
    private readonly IPostCacheKeyFactory _postCacheKeyFactory;
    private readonly TimeSpan _feedBuildingTTL = TimeSpan.FromMinutes(5);

    public PostCacheService(
        ICacheService cacheService,
        IPostCacheKeyFactory postCacheKeyFactory)
    {
        _cacheService = cacheService;
        _postCacheKeyFactory = postCacheKeyFactory;
    }

    public async Task<List<FeedItemModel>> GetProfileFeed(Guid profileId, long start, long stop, CancellationToken cancellationToken = default)
    {
        var key = _postCacheKeyFactory.GetProfileFeedKey(profileId);

        var result = await _cacheService.GetSortedSetRangeByRankWithScoresAsync(key, start, stop, cancellationToken);

        return result.Select(r =>
        {
            Guid postId = Guid.Empty;
            Guid.TryParse(r.Key, out postId);

            return new FeedItemModel(postId, r.Value);
        }).ToList();
    }

    public async Task<Result> SetProfileFeedAsync(Guid profileId, List<FeedItemModel> feedItems, CancellationToken cancellationToken = default)
    {
        string key = _postCacheKeyFactory.GetProfileFeedKey(profileId);

        IEnumerable<(string Member, double Score)> result =
            feedItems.Select(x => (Member: x.PostId.ToString(), Score: x.Score));

        await _cacheService.AddRangeToSortedSetAsync(key, result, cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<bool> TryLockFeedBuildingAsync(Guid profileId)
    {
        string feedBuildKey = _postCacheKeyFactory.GetFeedBuildingKey(profileId);

        return await _cacheService.SetIfNotExistsAsync(feedBuildKey, true, _feedBuildingTTL);
    }

    public async Task UnlockFeedBuildingAsync(Guid profileId)
    {
        string feedBuildKey = _postCacheKeyFactory.GetFeedBuildingKey(profileId);

        await _cacheService.RemoveAsync(feedBuildKey);
    }
}

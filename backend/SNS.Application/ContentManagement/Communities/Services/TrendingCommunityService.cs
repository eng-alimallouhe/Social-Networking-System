using SNS.Application.Abstractions.Caching;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Domain.Search.Documents;

namespace SNS.Application.ContentManagement.Communities.Services;

public class TrendingCommunityService
{
    private readonly ICacheService _cacheService;
    private readonly ICommunityCacheKeyFactory _communityCacheKeyFactory;
    private readonly ICommunitySearchService _elasticService;



    public TrendingCommunityService(
        ICacheService cacheService,
        ICommunityCacheKeyFactory communityCacheKeyFactory,
        ICommunitySearchService elasticService)
    {
        _cacheService = cacheService;
        _communityCacheKeyFactory = communityCacheKeyFactory;
        _elasticService = elasticService;
    }


    public async Task TrackActivityAsync(Guid communityId, double scoreBoost, CancellationToken cancellationToken = default)
    {
        var key = _communityCacheKeyFactory.GetTrendingCommunitiesKey(DateTime.UtcNow);

        await _cacheService.IncrementSortedSetScoreAsync(key, communityId.ToString(), scoreBoost, cancellationToken);

        await _cacheService.TrimSortedSetAsync(key, 0, -101, cancellationToken);

        await _cacheService.SetKeyExpiryAsync(key, TimeSpan.FromDays(7), cancellationToken);
    }

    public async Task<List<CommunityDocument>> GetTrendingCommunitiesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var key = _communityCacheKeyFactory.GetTrendingCommunitiesKey(DateTime.UtcNow);

        var topCommunityIds = await _cacheService.GetTopSortedSetMembersAsync(key, count, cancellationToken);

        if (topCommunityIds.Length == 0)
            return new List<CommunityDocument>();

        var searchResult = await _elasticService.GetCommunitiesByIds(topCommunityIds.ToList(), count, cancellationToken);

        var sortedTrendingCommunities = topCommunityIds
            .Select(redisId => searchResult
                .Documents.FirstOrDefault(elasticDoc => elasticDoc.Id.ToString() == redisId))
            .Where(doc => doc != null)
            .ToList();

        return sortedTrendingCommunities!;
    }
}

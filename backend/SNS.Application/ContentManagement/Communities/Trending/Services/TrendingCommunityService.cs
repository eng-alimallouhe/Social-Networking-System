using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Caching;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Communities.Services;
using SNS.Application.ContentManagement.Communities.Trending.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Communities.Trending.Services;

/// <summary>
/// Implements community trending logic using Redis Sorted Sets and authoritative database queries.
/// </summary>
public class TrendingCommunityService : ITrendingCommunityService
{
    private readonly ICacheService _cacheService;
    private readonly ICommunityCacheKeyFactory _communityCacheKeyFactory;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public TrendingCommunityService(
        ICacheService cacheService,
        ICommunityCacheKeyFactory communityCacheKeyFactory,
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _cacheService = cacheService;
        _communityCacheKeyFactory = communityCacheKeyFactory;
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task TrackActivityAsync(Guid communityId, double scoreBoost, CancellationToken cancellationToken = default)
    {
        var key = _communityCacheKeyFactory.GetTrendingCommunitiesKey(DateTime.UtcNow);

        await _cacheService.IncrementSortedSetScoreAsync(key, communityId.ToString(), scoreBoost, cancellationToken);
        await _cacheService.TrimSortedSetAsync(key, 0, -101, cancellationToken);
        await _cacheService.SetKeyExpiryAsync(key, TimeSpan.FromDays(7), cancellationToken);
    }

    public async Task<List<CommunitySummaryDto>> GetTrendingCommunitiesAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        var key = _communityCacheKeyFactory.GetTrendingCommunitiesKey(DateTime.UtcNow);
        var topCommunityIds = await _cacheService.GetTopSortedSetMembersAsync(key, count, cancellationToken);

        if (topCommunityIds.Length == 0)
        {
            return new List<CommunitySummaryDto>();
        }

        var parsedGuids = topCommunityIds
            .Select(id => Guid.TryParse(id, out var g) ? (Guid?)g : null)
            .Where(g => g.HasValue)
            .Select(g => g!.Value)
            .ToList();

        if (parsedGuids.Count == 0)
        {
            return new List<CommunitySummaryDto>();
        }

        var rawList = await _dbContext.Communities
            .AsNoTracking()
            .Where(c => parsedGuids.Contains(c.Id) && c.IsActive)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Description,
                c.Type,
                c.LogoObjectKey,
                MembersCount = c.Memberships.Count(m => m.Status == CommunityMembershipStatus.Active),
                c.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var distinctKeys = rawList
            .Select(c => c.LogoObjectKey)
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

        var itemsMap = rawList.ToDictionary(
            c => c.Id,
            c => new CommunitySummaryDto(
                c.Id,
                c.Name,
                c.Description,
                c.Type,
                !string.IsNullOrWhiteSpace(c.LogoObjectKey) && urlMap.TryGetValue(c.LogoObjectKey, out var url) ? url : null,
                c.MembersCount,
                c.CreatedAt));

        var sortedTrending = parsedGuids
            .Where(id => itemsMap.ContainsKey(id))
            .Select(id => itemsMap[id])
            .ToList();

        return sortedTrending;
    }
}

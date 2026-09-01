using SNS.Application.Abstractions.Caching;
using SNS.Application.Projects.Abstractions;
using SNS.Application.Projects.Contracts;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Projects.Services;

public sealed class ProjectCacheService : IProjectCacheService
{
    private readonly ICacheService _cacheService;
    private readonly IProjectCacheKeyFactory _projectCacheKeyFactory;
    private readonly TimeSpan _feedTTL = TimeSpan.FromMinutes(15);

    public ProjectCacheService(
        ICacheService cacheService,
        IProjectCacheKeyFactory projectCacheKeyFactory)
    {
        _cacheService = cacheService;
        _projectCacheKeyFactory = projectCacheKeyFactory;
    }

    public async Task<List<ProjectFeedItemModel>> GetProfileFeedAsync(Guid profileId, long start, long stop, CancellationToken cancellationToken = default)
    {
        var key = _projectCacheKeyFactory.GetProjectProfileFeedKey(profileId);

        var result = await _cacheService.GetSortedSetRangeByRankWithScoresAsync(key, start, stop, cancellationToken);

        return result.Select(r =>
        {
            Guid projectId = Guid.Empty;
            Guid.TryParse(r.Key, out projectId);

            return new ProjectFeedItemModel(projectId, r.Value);
        }).ToList();
    }

    public async Task<Result> SetProfileFeedAsync(Guid profileId, List<ProjectFeedItemModel> feedItems, CancellationToken cancellationToken = default)
    {
        string key = _projectCacheKeyFactory.GetProjectProfileFeedKey(profileId);

        IEnumerable<(string Member, double Score)> result =
            feedItems.Select(x => (Member: x.ProjectId.ToString(), Score: x.Score));

        await _cacheService.AddRangeToSortedSetAsync(key, result, cancellationToken);
        await _cacheService.SetKeyExpiryAsync(key, _feedTTL, cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

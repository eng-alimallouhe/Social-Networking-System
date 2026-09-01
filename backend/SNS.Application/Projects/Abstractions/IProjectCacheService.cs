using SNS.Application.Projects.Contracts;
using SNS.Shared.Results;

namespace SNS.Application.Projects.Abstractions;

public interface IProjectCacheService
{
    Task<List<ProjectFeedItemModel>> GetProfileFeedAsync(Guid profileId, long start, long stop, CancellationToken cancellationToken = default);
    Task<Result> SetProfileFeedAsync(Guid profileId, List<ProjectFeedItemModel> feedItems, CancellationToken cancellationToken = default);
}

using SNS.Application.Projects.Contracts;
using SNS.Shared.Results;

namespace SNS.Application.Projects.Abstractions;

public interface IProjectFeedService
{
    Task<Result> GenerateAndCacheUserFeedAsync(Guid profileId, ProjectFeedParameter feedParams, CancellationToken cancellationToken = default);
}

using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Shared.Results;

namespace SNS.Application.Profiles.Profiles.abstractions;

public interface IProfileCacheService
{
    Task<ProfileIntegrationModel?> GetProfileAsync(Guid profileId, CancellationToken cancellationToken = default);
    Task<ProfileIntegrationModel?> GetProfileByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> SetProfileAsync(ProfileIntegrationModel model, CancellationToken cancellationToken = default);

    Task<Result> RemoveProfileAsync(Guid profileId, Guid userId, CancellationToken cancellationToken = default);
}

using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Shared.Results;

namespace SNS.Application.Identity.Shared.Abstractions;

public interface IUserCacheService
{
    Task<UserModel?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> SetUserAsync(UserModel userModel, CancellationToken cancellationToken = default);

    Task<Result> RemoveUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result> SetUserActivationChanlageAsync(Guid userId, string Token, CancellationToken cancellationToken = default);

    Task<Result> VerifyUserActivationChanlageAsync(Guid userId, string token, CancellationToken cancellationToken = default);

    Task<Result> CompleteUserActivationChanlageAsync(Guid userId, CancellationToken cancellationToken = default);
}

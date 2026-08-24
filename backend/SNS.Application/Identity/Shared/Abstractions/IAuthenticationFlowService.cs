using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Shared.Results;

namespace SNS.Application.Identity.Shared.Abstractions;

public interface IAuthenticationFlowService
{
    Task<Result<AuthTokensDto>> AuthenticateUserAsync(
        AuthenticateUserRequest authenticateUserRequest,
        CancellationToken cancellationToken = default);
}

using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Shared.Results;

namespace SNS.Application.Identity.SecuritySessions.Abstractions;

public interface IAuthResponseService
{
    Task<Result<AuthTokensDto>> GenerateAuthResponseAsync(AuthResponseGenerationDto dto, CancellationToken cancellationToken);
}

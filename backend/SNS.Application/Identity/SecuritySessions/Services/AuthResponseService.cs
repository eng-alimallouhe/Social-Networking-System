using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.SecuritySessions.Services;

public class AuthResponseService : IAuthResponseService
{
    private readonly ITokenService _tokenService;
    private readonly IProfileCacheService _profileCacheService;


    public AuthResponseService(
        ITokenService tokenService,
        IProfileCacheService profileCacheService)
    {
        _tokenService = tokenService;
        _profileCacheService = profileCacheService;
    }

    public async Task<Result<AuthTokensDto>> GenerateAuthResponseAsync(AuthResponseGenerationDto dto, CancellationToken cancellationToken)
    {
        var profileModel = await _profileCacheService.GetProfileByUserIdAsync(dto.UserId, cancellationToken);
        
        Guid? profileId = profileModel != null? profileModel.ProfileId : null;

        var accessToken = _tokenService.GenerateAccessToken(new AccessTokenCreateDto(
            dto.UserId, dto.RoleId, profileId, dto.RoleType, dto.SessionId));

        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(dto.SessionId, cancellationToken);

        if (refreshToken == null || accessToken == null) 
            return Result<AuthTokensDto>.Failure(OperationStatusCode.ServerError);

        return Result<AuthTokensDto>.Success(new AuthTokensDto
        (
            Token: accessToken,
            RefreshToken: refreshToken
        ), OperationStatusCode.Success);
    }
}

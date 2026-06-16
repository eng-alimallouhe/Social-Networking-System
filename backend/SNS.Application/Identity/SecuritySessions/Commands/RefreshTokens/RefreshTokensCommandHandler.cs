using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Commands.RefreshTokens;

public class RefreshTokensCommandHandler : ICommandHandler<RefreshTokensCommand, AuthTokensDto>
{
    private readonly IUserCacheService _userCacheService;
    private readonly IRepository<RefreshToken> _refreshTokenRepo;
    private readonly ISessionService _sessionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IProfileCacheService _profileCacheService;

    public RefreshTokensCommandHandler(
        IUserCacheService userCacheService,
        ISessionService sessionService,
        IRepository<RefreshToken> refreshTokenRepo,
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        ICurrentUserService currentUserService,
        IProfileCacheService profileCacheService)
    {
        _userCacheService = userCacheService;
        _sessionService = sessionService;
        _refreshTokenRepo = refreshTokenRepo;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _currentUserService = currentUserService;
        _profileCacheService = profileCacheService;
    }


    public async Task<Result<AuthTokensDto>> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var sessionId = _currentUserService.SessionId;
        var profileId = _currentUserService.ProfileId;

        if (userId == null || sessionId == null)
            return Result<AuthTokensDto>.Failure(OperationStatusCode.AuthenticationRequired);

        if (profileId == null)
            return Result<AuthTokensDto>.Failure(UserStatusCodes.ProfileNotCompleted);

        var refreshToken = await _refreshTokenRepo.GetSingleByExpressionAsync(
            rt => rt.Token == request.RefreshToken && rt.SecuritySessionId == sessionId.Value, cancellationToken);

        if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
            return Result<AuthTokensDto>.Failure(UserStatusCodes.Unauthorized);
        
        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            if (refreshToken.IsRevoked || refreshToken.IsUsed)
            {
                await _sessionService.KillSwitchSessionAsync(sessionId.Value, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(UserStatusCodes.Unauthorized);
            }

            var userModel = await _userCacheService.GetUserAsync(userId.Value, cancellationToken);
            var profileModel = await _profileCacheService.GetProfileAsync(profileId.Value, cancellationToken);

            if (userModel == null)
                return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);

            if (profileModel == null) 
                return Result<AuthTokensDto>.Failure(UserStatusCodes.ProfileNotCompleted);

            if (userModel.Status == UserStatus.Suspended || userModel.Status == UserStatus.PermanentlyBanned || !profileModel.IsActive)
            {
                await _sessionService.KillSwitchSessionAsync(sessionId.Value, cancellationToken);
                await _unitOfWork.CompleteAsync(cancellationToken);
                return Result<AuthTokensDto>.Failure(UserStatusCodes.Suspended);
            }

            var accessTokenDto = new AccessTokenCreateDto(userModel.UserId, userModel.RoleId, profileId.Value, userModel.RoleType, sessionId.Value);
            var accessToken = _tokenService.GenerateAccessToken(accessTokenDto);
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(sessionId.Value, cancellationToken);

            refreshToken.Revoke();
            refreshToken.Use();

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<AuthTokensDto>.Success(
                new AuthTokensDto(
                    accessToken, 
                    newRefreshToken), UserStatusCodes.Found);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

    }
}

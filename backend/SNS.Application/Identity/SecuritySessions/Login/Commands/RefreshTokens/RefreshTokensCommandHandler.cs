using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.SecuritySessions.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.RefreshTokens;

public sealed class RefreshTokensCommandHandler : ICommandHandler<RefreshTokensCommand, AuthTokensDto>
{
    private readonly ITokenService _tokenService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<SecuritySession> _sessionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserCacheService _userCacheService;
    private readonly IProfileCacheService _profileCacheService;
    private readonly IGeneratorService _generatorService;

    public RefreshTokensCommandHandler(
        ITokenService tokenService, 
        ICurrentUserService currentUserService, 
        IRepository<SecuritySession> sessionRepo,
        IUnitOfWork unitOfWork,
        IUserCacheService userCacheService,
        IProfileCacheService profileCacheService,
        IGeneratorService generatorService)
    {
        _tokenService = tokenService;
        _currentUserService = currentUserService;
        _sessionRepo = sessionRepo;
        _unitOfWork = unitOfWork;
        _userCacheService = userCacheService;
        _profileCacheService = profileCacheService;
        _generatorService = generatorService;
    }

    public async Task<Result<AuthTokensDto>> Handle(RefreshTokensCommand request, CancellationToken cancellationToken)
    {
        var currentSessionId = _currentUserService.SessionId;
        var userId = _currentUserService.UserId;

        if (userId == null || currentSessionId == null)
        {
            return Result<AuthTokensDto>.Failure(SecurityStatusCodes.UnAuthorized);
        }

        var spec = new SessionByTokenOrIdSpecification(currentSessionId, request.refreshToken);
        
        var session = await _sessionRepo.GetSingleAsync(spec, cancellationToken);

        if (session == null || !session.IsActive || session.IsRevoked || session.LogoutAt != null)
        {
            return Result<AuthTokensDto>.Failure(SecurityStatusCodes.UnAuthorized);
        }

        var userCache = await _userCacheService.GetUserAsync(userId.Value, cancellationToken);
        
        var profileCache = await _profileCacheService.GetProfileByUserIdAsync(userId.Value, cancellationToken);

        if (userCache == null || profileCache == null)
        {
            return Result<AuthTokensDto>.Failure(SecurityStatusCodes.UnAuthorized);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {

            var newAccessToken = _tokenService.GenerateAccessToken(new AccessTokenCreateDto(
                UserId: userId.Value, 
                ProfileId: profileCache.ProfileId,
                RoleType: userCache.RoleType,
                SessionId: currentSessionId.Value));
            
            var refreshToken = _generatorService.GenerateSecureString();

            session.UpdateRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));

            await _unitOfWork.CompleteAsync(cancellationToken);
            
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result<AuthTokensDto>.Success(
                new AuthTokensDto(
                    Token: newAccessToken, RefreshToken: refreshToken),
                OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
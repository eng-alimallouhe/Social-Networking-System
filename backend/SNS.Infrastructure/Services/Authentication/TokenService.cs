using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNS.Application.Abstractions.Authentication;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Security;
using SNS.Application.DTOs.Authentication.Common.Responses;
using SNS.Application.Settings;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Common;
using SNS.Common.StatusCodes.Security;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Security.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SNS.Infrastructure.Services.Authentication;

/// <summary>
/// Represents the implementation of the token service responsible for
/// the generation, management, and rotation of authentication tokens.
/// 
/// This service encapsulates the business logic related to
/// JWT creation, cryptographic refresh token generation, and the secure
/// refreshing of user sessions.
/// </summary>
public class TokenService : ITokenService
{
    private readonly JWTSettings _jwtSettings;
    private readonly ITokenReaderService _tokenReader;
    private readonly IRepository<RefreshToken> _refreshTokenRepo;
    private readonly ISoftDeletableRepository<User> _userRepo;
    private readonly IUserSessionService _sessionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISessionService _sessionChecker;
    private readonly IGeneratorService _generatorService;

    public TokenService(
        IOptions<JWTSettings> jwtSettings,
        ITokenReaderService tokenReader,
        IRepository<RefreshToken> refreshTokenRepo,
        ISoftDeletableRepository<User> userRepo,
        IUserSessionService sessionService,
        IUnitOfWork unitOfWork,
        IGeneratorService generatorService,
        ISessionService sessionChecker)
    {
        _jwtSettings = jwtSettings.Value;
        _tokenReader = tokenReader;
        _refreshTokenRepo = refreshTokenRepo;
        _userRepo = userRepo;
        _sessionService = sessionService;
        _unitOfWork = unitOfWork;
        _generatorService = generatorService;
        _sessionChecker = sessionChecker;
    }

    public string GenerateAccessToken(User user, Guid sessionId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            
            // The 'sid' claim is critical for the SessionMiddleware to validate 
            // that the user's session exists in the distributed cache (Redis)
            // and has not been revoked.
            new("sid", sessionId.ToString()),
            new(ClaimTypes.Role, user.Role?.Type.ToString() ?? "User"),
            new("profileId", user.Profile?.Id.ToString() ?? string.Empty)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public async Task<string> GrantRefreshTokenAsync(User user)
    {
        var newTokenString = _generatorService.GenerateSecureString();

        var newToken = new RefreshToken
        {
            UserId = user.Id,
            Token = newTokenString,
        };

        await _refreshTokenRepo.AddAsync(newToken);

        // We do not call SaveChanges here because this method is typically part 
        // of a larger transaction (like Login or Register) orchestrated by the caller.
        return newTokenString;
    }

    public async Task<Result<AuthTokensDto>> RefreshTokensAsync(string oldAccessToken, string incomingRefreshToken)
    {
        var userId = _tokenReader.GetUserIdFromToken(oldAccessToken);
        if (userId == null) return Result<AuthTokensDto>.Failure(VerificationStatusCodes.InvalidCode);

        // We track the entity here because we intend to update the token string and expiration
        // (Token Rotation) rather than deleting and re-inserting.
        var storedRefreshToken = await _refreshTokenRepo.GetSingleByExpressionAsync(
            rt => rt.Token == incomingRefreshToken && rt.UserId == userId.Value,
            isTrackingEnable: true);

        if (storedRefreshToken == null) return Result<AuthTokensDto>.Failure(VerificationStatusCodes.InvalidCode);

        if (storedRefreshToken.ExpiresAt < DateTime.UtcNow) return Result<AuthTokensDto>.Failure(VerificationStatusCodes.CodeExpired);

        var user = await _userRepo.GetByIdAsync(userId.Value);
        if (user == null) return Result<AuthTokensDto>.Failure(UserStatusCodes.NotFound);

        // ------------------------------------------------------------------
        // Session Recovery Logic
        // ------------------------------------------------------------------

        Guid finalSessionId;

        // Try to extract the Session ID from the EXPIRED Access Token
        var oldSessionId = _tokenReader.GetSessionIdFromToken(oldAccessToken);
        var isSessionAlive = false;

        if (oldSessionId.HasValue)
        {
            // We check if the session is still valid in the cache. 
            // If valid, this method also refreshes the 'LastSeenAt' timestamp.
            isSessionAlive = await _sessionChecker.ValidateAndUpdateSessionAsync(oldSessionId.Value);
        }

        if (isSessionAlive)
        {
            // The session is still valid; reuse it to maintain continuity in analytics/tracking.
            finalSessionId = oldSessionId!.Value;
        }
        else
        {
            // The session expired in Redis or was lost. We create a NEW session transparently
            // so the user does not experience a forced logout (Hybrid Flow).
            var sessionResult = await _sessionService.CreateSessionAsync(userId.Value);

            if (!sessionResult.IsSuccess)
                return Result<AuthTokensDto>.Failure(sessionResult.StatusCode);

            finalSessionId = sessionResult.Value;
        }

        // ------------------------------------------------------------------

        // Rotate Refresh Token
        // This invalidates the old string immediately, preventing replay attacks if the old token was stolen.
        var newRefreshTokenString = _generatorService.GenerateSecureString();
        storedRefreshToken.Token = newRefreshTokenString;
        storedRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        storedRefreshToken.CreatedAt = DateTime.UtcNow;

        var newAccessToken = GenerateAccessToken(user, finalSessionId);

        await _unitOfWork.CompleteAsync();

        return Result<AuthTokensDto>.Success(
            new AuthTokensDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshTokenString
            },
            VerificationStatusCodes.CodeVerified);
    }

    public async Task<Result> ClearRefreshTokensAsync(User user)
    {
        var currentRefreshTokens = await _refreshTokenRepo.GetListByExpressionAsync(
            rt => rt.UserId == user.Id);

        // This effectively implements "Sign Out Everywhere" by revoking all long-lived credentials.
        await _refreshTokenRepo.DeleteRangeAsync(currentRefreshTokens);

        return Result.Success(OperationStatusCode.Success);
    }
}
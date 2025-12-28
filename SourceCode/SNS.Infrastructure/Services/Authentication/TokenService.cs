using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNS.Application.Abstractions.Authentication;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Security;
using SNS.Application.DTOs.Authentication.Responses;
using SNS.Application.Settings;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Security;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Security.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SNS.Infrastructure.Services.Authentication;

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

    // ========================================================================
    // 1. Generate Access Token
    // ========================================================================
    public string GenerateAccessToken(User user, Guid sessionId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            // Standard Claims
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            
            // Custom Claims
            // 'sid' is critical for the SessionMiddleware to validate user presence in Redis
            new("sid", sessionId.ToString()),
            new(ClaimTypes.Role, user.Role?.Type ?? "User"), 
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

    // ========================================================================
    // 2. Grant Refresh Token (Login Flow)
    // ========================================================================
    public async Task<string> GrantRefreshTokenAsync(User user)
    {
        // Use the centralized generator service for cryptographic randomness
        var newTokenString = _generatorService.GenerateSecureString();
        var expiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        // Fetch existing token with tracking enabled to allow direct updates
        var existingToken = await _refreshTokenRepo.GetSingleByExpressionAsync(
            rt => rt.UserId == user.Id,
            isTrackingEnable: true);

        if (existingToken != null)
        {
            // Update Strategy: Modify the existing record to prevent index fragmentation
            // and reduce I/O overhead compared to Delete/Insert.
            existingToken.Token = newTokenString;
            existingToken.ExpiresAt = expiresAt;
            existingToken.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            // Insert Strategy: First time login or previous token was manually revoked
            var newToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newTokenString,
                ExpiresAt = expiresAt,
                CreatedAt = DateTime.UtcNow
            };

            await _refreshTokenRepo.AddAsync(newToken);
        }

        // Note: SaveChanges is deferred to the caller (Orchestrator/LoginService)
        return newTokenString;
    }

    // ========================================================================
    // 3. Refresh Process (Rotation + Session Renewal)
    // ========================================================================
    public async Task<Result<AuthenticationResultDto>> RefreshTokensAsync(string oldAccessToken, string incomingRefreshToken)
    {
        // A. Extract User ID
        var userId = _tokenReader.GetUserIdFromToken(oldAccessToken);
        if (userId == null) return Result<AuthenticationResultDto>.Failure(VerificationStatusCodes.InvalidCode);

        // B. Fetch Refresh Token
        var storedRefreshToken = await _refreshTokenRepo.GetSingleByExpressionAsync(
            rt => rt.Token == incomingRefreshToken && rt.UserId == userId.Value,
            isTrackingEnable: true);

        if (storedRefreshToken == null) return Result<AuthenticationResultDto>.Failure(VerificationStatusCodes.InvalidCode);

        if (storedRefreshToken.ExpiresAt < DateTime.UtcNow) return Result<AuthenticationResultDto>.Failure (VerificationStatusCodes.CodeExpired);

        // C. Hydrate User
        var user = await _userRepo.GetByIdAsync(userId.Value);
        if (user == null) return Result<AuthenticationResultDto>.Failure(UserStatusCodes.UserNotFound);

        // ====================================================================
        // D. Session Handling Logic (Smart Reuse)
        // ====================================================================

        Guid finalSessionId;

        // 1. Try to extract the Session ID from the EXPIRED Access Token
        var oldSessionId = _tokenReader.GetSessionIdFromToken(oldAccessToken);
        var isSessionAlive = false;

        if (oldSessionId.HasValue)
        {
            // 2. Check if this session is still active in Redis
            // We use the lightweight service. If it returns true, it also updates LastSeenAt.
            isSessionAlive = await _sessionChecker.ValidateAndUpdateSessionAsync(oldSessionId.Value);
        }

        if (isSessionAlive)
        {
            // CASE 1: Session is valid. Reuse it to keep analytics clean.
            finalSessionId = oldSessionId!.Value;
        }
        else
        {
            // CASE 2: Session is missing/expired in Redis. Create a FRESH one.
            // This covers the "Hybrid Flow" where we recover from a lost session.
            var sessionResult = await _sessionService.CreateSessionAsync(userId.Value);

            if (!sessionResult.IsSuccess)
                return Result<AuthenticationResultDto>.Failure(sessionResult.StatusCode);

            finalSessionId = sessionResult.Value;
        }

        // ====================================================================

        // E. Rotate Refresh Token
        var newRefreshTokenString = _generatorService.GenerateSecureString();
        storedRefreshToken.Token = newRefreshTokenString;
        storedRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        storedRefreshToken.CreatedAt = DateTime.UtcNow;

        // F. Generate New Access Token with the DETERMINED Session ID
        var newAccessToken = GenerateAccessToken(user, finalSessionId);

        // G. Commit
        await _unitOfWork.CompleteAsync();

        return Result<AuthenticationResultDto>.Success(
            new AuthenticationResultDto
            {
                Token = newAccessToken,
                RefreshToken = newRefreshTokenString
            },
            VerificationStatusCodes.CodeVerified);
    }
}
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNS.Application.Abstractions.Common;
using SNS.Application.Identity.SecuritySessions.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Shared.Settings;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SNS.Infrastructure.Identity.Shared.Services;

public class TokenService : ITokenService
{
    private readonly JWTSettings _jwtSettings;
    private readonly IRepository<RefreshToken> _refreshTokenRepo;
    private readonly IGeneratorService _generatorService;
    

    public TokenService(
        IOptions<JWTSettings> jwtSettings,
        IRepository<RefreshToken> refreshTokenRepo,
        IGeneratorService generatorService)
    {
        _jwtSettings = jwtSettings.Value;
        _refreshTokenRepo = refreshTokenRepo;
        _generatorService = generatorService;
    }

    public string GenerateAccessToken(AccessTokenCreateDto dto)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, dto.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("sid", dto.SessionId.ToString()),
            new Claim(ClaimTypes.Role, dto.RoleType.ToString()),
            new Claim("profileId", dto.ProfileId.ToString() ?? string.Empty)
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

    public async Task<string> GenerateRefreshTokenAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        var newTokenString = _generatorService.GenerateSecureString();

        var newToken = RefreshToken.Create(securitySessionId: sessionId, token: newTokenString);
        newToken.SetExpiration(expiresAt: DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays));

        await _refreshTokenRepo.AddAsync(newToken, cancellationToken);
        return newTokenString;
    }
}

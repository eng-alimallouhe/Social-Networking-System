using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNS.Application.Abstractions.Common;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Shared.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SNS.Infrastructure.Identity.Shared.Services;

public class TokenService : ITokenService
{
    private readonly JWTSettings _jwtSettings;
    private readonly IGeneratorService _generatorService;
    

    public TokenService(
        IOptions<JWTSettings> jwtSettings,
        IGeneratorService generatorService)
    {
        _jwtSettings = jwtSettings.Value;
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
            Expires = DateTime.UtcNow.AddYears(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = credentials
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
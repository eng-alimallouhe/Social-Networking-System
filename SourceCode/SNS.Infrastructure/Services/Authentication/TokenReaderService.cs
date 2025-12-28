using Azure.Core;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SNS.Application.Abstractions.Authentication;
using SNS.Application.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SNS.Infrastructure.Services.Authentication;

public class TokenReaderService : ITokenReaderService
{
    private readonly JWTSettings _jwtSettings;

    public TokenReaderService(IOptions<JWTSettings> options)
    {
        _jwtSettings = options.Value;
    }


    public string? GetEmail(string accessToken)
    {
        var principal = GetPrincipal(accessToken);
        var userIdString = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return principal?.FindFirst(ClaimTypes.Email)?.Value;
    }

    public Guid? GetSessionIdFromToken(string accessToken)
    {
        var principal = GetPrincipal(accessToken);
        var sidString = principal?.FindFirst("sid")?.Value;

        return Guid.TryParse(sidString, out var sid) ? sid : null;
    }

    public Guid? GetUserIdFromToken(string accessToken)
    {
        var principal = GetPrincipal(accessToken);
        var userIdString = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdString, out var userId) ? userId : null;
    }

    private ClaimsPrincipal? GetPrincipal(string accessToken)
    {
        string secretKey = _jwtSettings.SecretKey;

        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.UTF8.GetBytes(secretKey);

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };

        try
        {
            var principal = tokenHandler.ValidateToken(accessToken, parameters, out SecurityToken validatedToken);

            Console.WriteLine("All Claims:");
            foreach (var claim in principal.Claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }


            return principal;
        }
        catch
        {
            return null;
        }
    }
}
using System.IdentityModel.Tokens.Jwt;
using SNS.Application.Identity.Shared.Abstractions;

namespace SNS.Infrastructure.Identity.Shared.Services;

/// <summary>
/// Implementation of ITokenReaderService that extracts information from JWT tokens.
/// </summary>
public class TokenReaderService : ITokenReaderService
{
    public string? GetEmail(string accessToken)
    {
        try 
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetUserIdFromToken(string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);
            var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(sub, out var userId) ? userId : null;
        }
        catch
        {
            return null;
        }
    }

    public Guid? GetSessionIdFromToken(string accessToken)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);
            var sid = jwtToken.Claims.FirstOrDefault(c => c.Type == "sid")?.Value;
            return Guid.TryParse(sid, out var sessionId) ? sessionId : null;
        }
        catch
        {
            return null;
        }
    }
}

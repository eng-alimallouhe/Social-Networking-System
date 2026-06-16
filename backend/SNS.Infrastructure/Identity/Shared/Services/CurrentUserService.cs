using Microsoft.AspNetCore.Http;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.Users.Enums;
using System.Security.Claims;

namespace SNS.Infrastructure.Identity.Shared.Services;

/// <summary>
/// Represents the implementation of the current user service based on the HTTP Context.
/// 
/// This service acts as an adapter, translating the ASP.NET Core <see cref="HttpContext"/>
/// and <see cref="ClaimsPrincipal"/> into domain-specific properties.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Checks "sub" (standard JWT) first, then falls back to NameIdentifier (Microsoft mapping)
    // to ensure compatibility regardless of how the token handler mapped the claims.
    public Guid? UserId => GetGuidClaim("sub") ?? GetGuidClaim(ClaimTypes.NameIdentifier);

    public Guid? SessionId => GetGuidClaim("sid");

    public Guid? ProfileId => GetGuidClaim("profileId");

    public string? RoleType =>
        _httpContextAccessor.HttpContext?
            .User?.FindFirstValue(ClaimTypes.Role);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? BearerToken
    {
        get
        {
            var authHeader = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            // We must manually strip the "Bearer " schema to get the raw token 
            // for use in downstream API calls or manual validation.
            return authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true
                ? authHeader["Bearer ".Length..].Trim()
                : null;
        }
    }


    private Guid? GetGuidClaim(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?
            .User?.FindFirst(claimType)?.Value;

        // Using TryParse prevents runtime exceptions if a claim is malformed or empty.
        return Guid.TryParse(value, out var guid) ? guid : null;
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using SNS.Domain.Identity.Users.Constants;

namespace SNS.Infrastructure.Identity.Shared.Authorization;

/// <summary>
/// Dynamically and safely resolves authorization policies matching known system permission requirement strings.
/// </summary>
public sealed class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
        : base(options)
    {
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);
        if (policy != null)
        {
            return policy;
        }

        // Only generate dynamic permission policies for known, valid application permissions from the catalog.
        // This ensures typos (e.g., "Support.Tickets.Replly") do not silently generate invalid policies.
        if (PermissionsCatalog.IsValidPermission(policyName))
        {
            return new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
        }

        return null;
    }
}

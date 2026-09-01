using Microsoft.AspNetCore.Authorization;

namespace SNS.Infrastructure.Identity.Shared.Authorization;

/// <summary>
/// Authorization requirement representing a specific system permission.
/// </summary>
public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SNS.Application.Identity.Shared.Abstractions;

namespace SNS.Infrastructure.Identity.Shared.Authorization;

/// <summary>
/// Authorizes HTTP requests by evaluating whether the authenticated user's assigned role possesses the required permission.
/// </summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;

    public PermissionAuthorizationHandler(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        var roleName = context.User.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrWhiteSpace(roleName))
        {
            return;
        }

        if (await _permissionService.HasPermissionAsync(roleName, requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}

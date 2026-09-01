namespace SNS.Application.Identity.Shared.Abstractions;

/// <summary>
/// Service contract for resolving and verifying permissions assigned to roles.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Determines whether the specified role possesses the requested permission.
    /// </summary>
    /// <param name="roleName">The name or string representation of the role type.</param>
    /// <param name="permission">The permission identifier string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if authorized; otherwise <c>false</c>.</returns>
    Task<bool> HasPermissionAsync(string roleName, string permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all permissions currently assigned to the given role.
    /// </summary>
    /// <param name="roleName">The name or string representation of the role type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only set of permission identifier strings.</returns>
    Task<IReadOnlySet<string>> GetPermissionsForRoleAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invalidates cached role permissions across the distributed cache.
    /// </summary>
    Task InvalidateCacheAsync(CancellationToken cancellationToken = default);
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Identity.Users.Relations;
using SNS.Infrastructure.Persistence;

namespace SNS.Infrastructure.Identity.Users.Seeding;

/// <summary>
/// Authoritative, idempotent database seeder and synchronizer for system permissions and role-permission mappings.
/// </summary>
public static class PermissionSeeder
{
    public static async Task SeedPermissionsAndRolesAsync(
        SNSDbContext dbContext,
        IPermissionService? permissionService = null,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        logger?.LogInformation("Starting authoritative permission and role-permission synchronization...");

        // 1. Ensure all defined catalog permissions exist in database
        var existingPermissions = await dbContext.Permissions
            .ToDictionaryAsync(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var addedPermissionsCount = 0;
        foreach (var def in PermissionsCatalog.All)
        {
            if (!existingPermissions.TryGetValue(def.Name, out _))
            {
                var permission = Permission.Create(def.Name, def.Description);
                dbContext.Permissions.Add(permission);
                existingPermissions[def.Name] = permission;
                addedPermissionsCount++;
            }
        }

        if (addedPermissionsCount > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger?.LogInformation("Created {Count} missing permissions in database.", addedPermissionsCount);
        }

        // 2. Resolve existing roles and synchronize their role-permission assignments
        var existingRoles = await dbContext.Roles
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync(cancellationToken);

        var addedRolePermissionsCount = 0;
        var removedRolePermissionsCount = 0;

        foreach (var role in existingRoles)
        {
            var targetPermissionNames = GetTargetPermissionsForRole(role.Type);

            // A. Remove obsolete RolePermission mappings that are no longer in the catalog for this role
            var currentRolePermissions = role.RolePermissions.ToList();
            foreach (var rp in currentRolePermissions)
            {
                if (rp.Permission != null && !targetPermissionNames.Contains(rp.Permission.Name))
                {
                    dbContext.RolePermissions.Remove(rp);
                    removedRolePermissionsCount++;
                }
            }

            // B. Add missing RolePermission mappings defined in the catalog
            var assignedPermissionIds = currentRolePermissions
                .Where(rp => rp.Permission != null && targetPermissionNames.Contains(rp.Permission.Name))
                .Select(rp => rp.PermissionId)
                .ToHashSet();

            foreach (var permName in targetPermissionNames)
            {
                if (existingPermissions.TryGetValue(permName, out var permission))
                {
                    if (!assignedPermissionIds.Contains(permission.Id))
                    {
                        var rolePermission = RolePermission.Create(role.Id, permission.Id);
                        dbContext.RolePermissions.Add(rolePermission);
                        assignedPermissionIds.Add(permission.Id);
                        addedRolePermissionsCount++;
                    }
                }
            }
        }

        if (addedRolePermissionsCount > 0 || removedRolePermissionsCount > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger?.LogInformation(
                "Role-permission synchronization completed. Added: {Added}, Removed obsolete: {Removed}.",
                addedRolePermissionsCount,
                removedRolePermissionsCount);

            if (permissionService != null)
            {
                await permissionService.InvalidateCacheAsync(cancellationToken);
            }
        }
        else
        {
            logger?.LogInformation("Role-permission mappings are already up to date.");
        }

        logger?.LogInformation("Permission and role-permission synchronization finished successfully.");
    }

    private static IReadOnlySet<string> GetTargetPermissionsForRole(RoleType roleType)
    {
        return roleType switch
        {
            RoleType.Admin => PermissionsCatalog.AdminPermissions,
            RoleType.Moderator => PermissionsCatalog.ModeratorPermissions,
            RoleType.Support => PermissionsCatalog.SupportPermissions,
            RoleType.User => PermissionsCatalog.UserPermissions,
            RoleType.Guest => PermissionsCatalog.GuestPermissions,
            RoleType.Ghost => new HashSet<string>(),
            _ => new HashSet<string>()
        };
    }
}

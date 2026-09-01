using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.Users.Relations;

/// <summary>
/// Represents the junction entity mapping roles to permissions.
/// </summary>
public class RolePermission : Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid PermissionId { get; private set; }

    // Navigation
    public Role Role { get; private set; } = null!;
    public Permission Permission { get; private set; } = null!;

    private RolePermission()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static RolePermission Create(Guid roleId, Guid permissionId)
    {
        return new RolePermission
        {
            RoleId = roleId,
            PermissionId = permissionId
        };
    }
}

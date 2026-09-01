using SNS.Domain.Identity.Users.Relations;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.Users.Entities;

/// <summary>
/// Represents an application authorization permission.
/// </summary>
public class Permission : Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Navigation
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    private Permission()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static Permission Create(string name, string description)
    {
        return new Permission
        {
            Name = name,
            Description = description
        };
    }
}

using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.Users.Entities;

public class Role : Entity, ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    public RoleType Type { get; private set; }

    // Soft Delete
    public bool IsActive { get; private set; } = true;

    private Role()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsActive = true;
    }

    public static Role Create(RoleType type)
    {
        var entity = new Role();
        entity.Type = type;
        return entity;
    }

    public static Role CreateSystemUserRole(Guid id)
    {
        var entity = new Role();
        entity.Id = id;
        entity.Type = RoleType.SystemUser;
        return entity;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }
}

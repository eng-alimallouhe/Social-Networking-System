using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.ArchiveManagement.Entities;

public class IdentityArchive : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; } 


    // Foreign Key: One(User) To Many(IdentityArchives)
    public Guid UserId { get; private set; }


    public string OldUserIdentifier { get; private set; } = string.Empty;
    public string NewUserIdentifier { get; private set; } = string.Empty;
    public IdentityType Type { get; private set; }


    // Timestamp
    public DateTime CreatedAt { get; private set; }

    private IdentityArchive()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static IdentityArchive Create(Guid userId, string oldUserIdentifier, string newUserIdentifier, IdentityType type)
    {
        var entity = new IdentityArchive();
        entity.UserId = userId;
        entity.OldUserIdentifier = oldUserIdentifier;
        entity.NewUserIdentifier = newUserIdentifier;
        entity.Type = type;
        return entity;
    }
}

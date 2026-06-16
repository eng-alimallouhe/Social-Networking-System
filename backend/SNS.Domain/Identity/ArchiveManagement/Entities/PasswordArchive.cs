using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.ArchiveManagement.Entities;

public class PasswordArchive : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(User) To Many(PasswordArchives)
    public Guid UserId { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }

    private PasswordArchive()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static PasswordArchive Create(Guid userId)
    {
        var entity = new PasswordArchive();
        entity.UserId = userId;
        return entity;
    }
}

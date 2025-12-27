using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Security.Entities;

public class PasswordArchive : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(User) To Many(PasswordArchives)
    public Guid UserId { get; set; }

    public required string HashedPassword { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;

    public PasswordArchive()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
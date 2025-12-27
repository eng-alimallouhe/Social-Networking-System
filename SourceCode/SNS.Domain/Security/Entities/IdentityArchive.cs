using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Security.Enums;

namespace SNS.Domain.Security.Entities;

public class IdentityArchive : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; } 


    // Foreign Key: One(User) To Many(IdentityArchives)
    public Guid UserId { get; set; }


    public string UserIdentifier { get; set; } = string.Empty;
    public IdentityType Type { get; set; }


    // Timestamp
    public DateTime CreatedAt { get; set; }


    // Navigation
    public User User { get; set; } = null!;

    public IdentityArchive()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
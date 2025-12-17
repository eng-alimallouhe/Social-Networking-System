
using SNS.Domain.Security.Enums;

namespace SNS.Domain.Security;

public class IdentityArchive
{
    // Primary Key
    public Guid Id { get; set; } 

    // Foreign Key: One(User) To Many(IdentityArchives)
    public Guid UserId { get; set; }

    public required string UserIdentifier { get; set; }
    public IdentityType IdentityType { get; set; }

    // Timestamp
    public DateTime ChangedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}

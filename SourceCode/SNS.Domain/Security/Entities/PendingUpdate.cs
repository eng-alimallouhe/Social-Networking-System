using SNS.Domain.Abstractions.Common;
using SNS.Domain.Security.Enums;

namespace SNS.Domain.Security;

public class PendingUpdate : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }


    // Foreign Key: One(User) To Many(PendingUpdates)
    public Guid UserId { get; set; }


    public required string UpdatedInfo { get; set; }
    public UpdateType UpdateType { get; set; }


    // Timestamp
    public DateTime RequestedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}

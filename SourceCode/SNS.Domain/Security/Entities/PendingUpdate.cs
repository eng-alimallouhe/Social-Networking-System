using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Security.Enums;

namespace SNS.Domain.Security.Entities;

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
    public VerificationCode? VerificationCode { get; set; }

    public PendingUpdate()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        RequestedAt = DateTime.UtcNow;
    }
}
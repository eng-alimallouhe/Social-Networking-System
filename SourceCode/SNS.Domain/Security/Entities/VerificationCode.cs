
using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Security.Entities;

public class VerificationCode : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }


    // Foreign Key: One(User) To Many(VerificationCodes)
    public Guid UserId { get; set; }

    // Foreign Key: One(PendingUpdates) To One(VerificationCode)
    public Guid? PendingUpdateId { get; set; }

    public required string Code { get; set; }
    public CodeType Type { get; set; }
    public int FailedAttempts { get; set; } = 0;


    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }


    public bool IsUsed { get; set; } 

    // Navigation
    public User User { get; set; } = null!;
    public PendingUpdate? PendingUpdate { get; set; }


    public VerificationCode()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
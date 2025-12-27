using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Security.Entities;

public class ManualRecoveryRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(User) To Many(ManualRecoveryRequests as CandidateUser)
    public Guid UserId { get; set; }

    public required string ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? ProvidedInfoJson { get; set; }

    public RecoveryStatus Status { get; set; }
    public string? ReviewerNotes { get; set; }

    // Foreign Key: One(User) To Many(ManualRecoveryRequests as Reviewer)
    public Guid? ReviewedBy { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }

    // Navigation Properties
    public User CandidateUser { get; set; } = null!;
    public User Reviewer { get; set; } = null!;

    public ManualRecoveryRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}

using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Common.Helpers;
using SNS.Domain.QA.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.QA.Bridges;

public class ProblemView : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Problem) → Many(ProblemViews)
    public Guid ProblemId { get; set; }

    // Foreign Key: One(Profile) → Many(ProblemViews)
    public Guid ViewerId { get; set; }

    // Timestamp
    public DateTime ViewedAt { get; set; }

    // Optional Info
    public DeviceType? DeviceType { get; set; }
    public string? IpHash { get; set; }
    public string? Country { get; set; }

    //Soft Delet:
    public bool IsActive { get; set; }

    // Navigation Properties
    public Problem Problem { get; set; } = null!;
    public Profile Viewer { get; set; } = null!;

    public ProblemView()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        ViewedAt = DateTime.UtcNow;
        IsActive = true;
    }
}

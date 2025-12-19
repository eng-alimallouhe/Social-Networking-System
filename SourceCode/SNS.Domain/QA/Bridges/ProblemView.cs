
using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.QA.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.QA.Bridges;

public class ProblemView : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Problem) → Many(ProblemViews)
    public Guid ProblemId { get; set; }

    // Foreign Key: One(Profile) → Many(ProblemViews)
    public Guid ViewerProfileId { get; set; }

    // Timestamp
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    // Optional Info
    public DeviceType? DeviceType { get; set; }
    public string? IpHash { get; set; }
    public string? Country { get; set; }

    // Navigation Properties
    public Problem Problem { get; set; } = null!;
    public Profile ViewerProfile { get; set; } = null!;
}

using SNS.Domain.Projects.Entities;
using SNS.Domain.Common.Enums;
using SNS.Domain.SocialGraph;
using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Projects.Bridges;

public class ProjectView : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProjectId { get; set; }
    public Guid ViewerProfileId { get; set; }

    // Timestamp
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    // Optional Info
    public DeviceType? DeviceType { get; set; }
    public string? IpHash { get; set; }
    public string? Country { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public Profile ViewerProfile { get; set; } = null!;
}

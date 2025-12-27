using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Projects.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Projects.Bridges;

public class ProjectView : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProjectId { get; set; }
    public Guid ViewerProfileId { get; set; }

    // Timestamp
    public DateTime ViewedAt { get; set; }

    // Optional Info
    public DeviceType? DeviceType { get; set; }
    public string? IpHash { get; set; }
    public string? Country { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public Profile ViewerProfile { get; set; } = null!;

    public ProjectView()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        ViewedAt = DateTime.UtcNow;
    }
}
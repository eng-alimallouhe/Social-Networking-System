using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Projects.Bridges;

public class ProjectView : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Keys
    public Guid ProjectId { get; private set; }
    public Guid ViewerId { get; private set; }

    // Timestamp
    public DateTime ViewedAt { get; private set; }

    // Optional Info
    public DeviceType? DeviceType { get; private set; }
    public string? IpHash { get; private set; }
    public string? Country { get; private set; }
    
    // Soft Delete:
    public bool IsActive { get; private set; }

    // Navigation



    private ProjectView()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        ViewedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static ProjectView Create(Guid projectId, Guid viewerId, DeviceType? deviceType, string? ipHash, string? country)
    {
        return new ProjectView
        {
            ProjectId = projectId,
            ViewerId = viewerId,
            DeviceType = deviceType,
            IpHash = ipHash,
            Country = country
        };
    }

    public void SoftDelete()
    {
        IsActive = false;
    }
}

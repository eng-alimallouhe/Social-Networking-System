using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Projects.Entities;

public class ProjectMilestone : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key
    public Guid ProjectId { get; set; }

    // General Properties
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;

    public ProjectMilestone()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
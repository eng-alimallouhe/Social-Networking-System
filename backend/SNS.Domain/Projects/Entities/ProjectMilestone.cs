using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Projects.Entities;

public class ProjectMilestone : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key
    public Guid ProjectId { get; private set; }

    // General Properties
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; private set; }

    private ProjectMilestone()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static ProjectMilestone Create(Guid projectId, string title, string description)
    {
        return new ProjectMilestone
        {
            ProjectId = projectId,
            Title = title,
            Description = description
        };
    }
}

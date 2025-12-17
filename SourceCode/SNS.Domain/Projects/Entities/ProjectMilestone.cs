namespace SNS.Domain.Projects.Entities;

public class ProjectMilestone
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key
    public Guid ProjectId { get; set; }

    // General Properties
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Timestamp
    public DateTime Date { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
}

using SNS.Domain.Common.Enums;


namespace SNS.Domain.Projects.Entities;

public class ProjectMedia
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Project) → Many(Media)
    public Guid ProjectId { get; set; }

    // General Properties
    public string MediaUrl { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public MediaType Type { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
}

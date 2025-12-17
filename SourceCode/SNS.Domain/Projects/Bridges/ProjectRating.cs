using SNS.Domain.Projects.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Projects.Bridges;

public class ProjectRating
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ReactingProfileId { get; set; }
    public Guid ProjectId { get; set; }

    // General
    public int RatingValue { get; set; }
    public string Comment { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Project Project { get; set; } = null!;
    public Profile User { get; set; } = null!;
}

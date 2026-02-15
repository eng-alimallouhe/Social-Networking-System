using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Projects.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Projects.Bridges;

public class ProjectRating : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid RaterId { get; set; }
    public Guid ProjectId { get; set; }

    // General
    public int RatingValue { get; set; }
    public string Comment { get; set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public Profile Rater { get; set; } = null!;

    public ProjectRating()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
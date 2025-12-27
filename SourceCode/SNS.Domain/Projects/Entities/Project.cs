using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Enums;
using SNS.Domain.SocialGraph;


namespace SNS.Domain.Projects.Entities;

public class Project : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(Projects)
    public Guid OwnerProfileId { get; set; }

    // General Properties
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string MainImageUrl { get; set; } = string.Empty;
    public string ReadmeContent { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string LiveDemoUrl { get; set; } = string.Empty;
    public ProjectType Type { get; set; }
    public ProjectStatus Status { get; set; }
    public bool IsActive { get; set; } = true;

    // Timestamp
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; } 

    // Navigation Properties
    public Profile OwnerProfile { get; set; } = null!;
    public ICollection<ProjectSkill> Skills { get; set; } = new List<ProjectSkill>();
    public ICollection<ProjectTag> Tags { get; set; } = new List<ProjectTag>();
    public ICollection<ProjectContributor> Contributors { get; set; } = new List<ProjectContributor>();
    public ICollection<ProjectMedia> Media { get; set; } = new List<ProjectMedia>();
    public ICollection<ProjectRating> Ratings { get; set; } = new List<ProjectRating>();
    public ICollection<ProjectMilestone> Milestones { get; set; } = new List<ProjectMilestone>();
    public ICollection<ProjectView> Views { get; set; } = new List<ProjectView>();


    public Project()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}

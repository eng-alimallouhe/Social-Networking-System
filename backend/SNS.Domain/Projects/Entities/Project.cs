using SNS.Domain.Shared.Helpers;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Enums;
using System.Security.Cryptography.X509Certificates;


namespace SNS.Domain.Projects.Entities;

public class Project : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) ? Many(Projects)
    public Guid OwnerId { get; private set; }

    // General Properties
    public string Title { get; private set; } = string.Empty;
    public string ShortDescription { get; private set; } = string.Empty;
    public string MainImageUrl { get; private set; } = string.Empty;
    public string ReadmeContent { get; private set; } = string.Empty;
    public string GitHubUrl { get; private set; } = string.Empty;
    public string LiveDemoUrl { get; private set; } = string.Empty;
    public ProjectType Type { get; private set; }
    public ProjectStatus Status { get; private set; }
    public bool IsActive { get; private set; } = true;
    public string? SourceCodeTree { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; } 
    public DateTime UpdatedAt { get; private set; } 


    public ICollection<ProjectSkill> Skills { get; private set; }
        = new List<ProjectSkill>();
    
    public ICollection<ProjectTag> Tags { get; private set; } 
        = new List<ProjectTag>();
    
    public ICollection<ProjectContributor> Contributors { get; private set; } 
        = new List<ProjectContributor>();
    
    public ICollection<ProjectMedia> Media { get; private set; } 
        = new List<ProjectMedia>();
    
    public ICollection<ProjectRating> Ratings { get; private set; } 
        = new List<ProjectRating>();
    
    public ICollection<ProjectMilestone> Milestones { get; private set; } 
        = new List<ProjectMilestone>();
    
    public ICollection<ProjectView> Views { get; private set; } 
        = new List<ProjectView>();
    
    public ICollection<SavedProject> Saves { get; private set; } 
        = new List<SavedProject>();


    private Project()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
        Status = ProjectStatus.InProgress;
    }

    public static Project Create(Guid ownerId, string title, string shortDescription, string mainImageUrl, string readmeContent, string gitHubUrl, string liveDemoUrl, ProjectType type, ProjectStatus status)
    {
        return new Project
        {
            OwnerId = ownerId,
            Title = title,
            ShortDescription = shortDescription,
            MainImageUrl = mainImageUrl,
            ReadmeContent = readmeContent,
            GitHubUrl = gitHubUrl,
            LiveDemoUrl = liveDemoUrl,
            Type = type,
            Status = status
        };
    }

    public void SoftDelete()
    {
        if (IsActive)
        {
            IsActive = false;
        }
    }

    public void MarkSourceCodeAsReady(string jsonTree)
    {
        this.SourceCodeTree = jsonTree;
    }
}



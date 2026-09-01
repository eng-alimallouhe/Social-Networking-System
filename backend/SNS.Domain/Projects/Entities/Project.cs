using SNS.Domain.Shared.Helpers;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Enums;
using System.Security.Cryptography.X509Certificates;

using SNS.Domain.Shared.Entities;

namespace SNS.Domain.Projects.Entities;

public class Project : Entity, ISoftDeletable
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

    public void UpdateReadmeContent(string readmeContent)
    {
        this.ReadmeContent = readmeContent;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(ProjectStatus status)
    {
        this.Status = status;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateBasicInfo(string title, string shortDescription)
    {
        this.Title = title;
        this.ShortDescription = shortDescription;
        this.UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string title, string shortDescription, string liveDemoUrl)
    {
        this.Title = title;
        this.ShortDescription = shortDescription;
        this.LiveDemoUrl = liveDemoUrl;
        this.UpdatedAt = DateTime.UtcNow;
    }
}
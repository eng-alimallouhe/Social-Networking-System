using SNS.Domain.Projects.Enums;

namespace SNS.Domain.Search.Documents;

public class ProjectDocument
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string LiveDemoUrl { get; set; } = string.Empty;
    public string ReadmeContent { get; set; } = string.Empty;
    public ProjectType Type { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> TopThreeSkills { get; set; } = new List<string>();
    public int SkillsCount { get; set; }
    public int ContributorsCount { get; set; }
    public List<ProjectContributorDocument> TopThreeContributors { get; set; } = new List<ProjectContributorDocument>();
    public decimal Rate { get; set; }
    public int SavesCount { get; set; }
    public int totalRates { get; set; }
}


public class ProjectContributorDocument
{
    public Guid Id { get; set; }
    public string ContributorProfilePictureUrl { get; set; } = string.Empty;
    public string ContributorFullName { get; set; } = string.Empty; 
}

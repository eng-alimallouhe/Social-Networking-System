using SNS.Domain.Projects.Enums;

namespace SNS.Domain.Search.Documents;

public class ProjectDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string ReadmeContent { get; set; } = string.Empty;
    public ProjectType Type { get; set; }
    public ProjectStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Skills { get; set; } = new List<string>();
    public List<string> Tags { get; set; } = new List<string>();
}
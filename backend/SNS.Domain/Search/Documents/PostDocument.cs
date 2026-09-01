namespace SNS.Domain.Search.Documents;

public class PostDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Topics { get; set; } = new List<string>();
    public List<string> Tags { get; set; } = new List<string>();
}
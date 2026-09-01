namespace SNS.Domain.Search.Documents;

public class ProfileDocument
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public string? Bio { get; set; }
    public List<string> Universities { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
    public List<string> Skills { get; set; } = new List<string>();
}
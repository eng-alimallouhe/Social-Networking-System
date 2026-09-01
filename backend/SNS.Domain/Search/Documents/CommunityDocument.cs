using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Domain.Search.Documents;

public class CommunityDocument
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public CommunityType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Domain.Search.Documents;

public class PostDocument
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorSpecialization { get; set; } = string.Empty;
    public string AuthorProfilePictureUrl { get; set; } = string.Empty;
    public Guid? CommunityId { get; set; }
    public CommunityType? CommunityType { get; set; }
    public string? CommunityName { get; set; }
    public string? CommunityLogoUrl { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastInteractedAt { get; set; } = null;
    public List<string> MediaUrls { get; set; } = new List<string>();
    public List<string> Topics { get; set; } = new List<string>();
    public List<string> Tags { get; set; } = new List<string>();
    public int CommentsCount { get; set; }
    public int ReactionsCount { get; set; }
    public int ViewsCount { get; set; }
    public int SavesCount { get; set; }
}

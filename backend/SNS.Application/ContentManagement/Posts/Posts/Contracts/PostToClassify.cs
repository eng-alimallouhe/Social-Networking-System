using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.ContentManagement.Posts.Posts.Contracts;

public sealed class PostToClassify
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorSpecialization { get; set; } = string.Empty;
    public string? AuthorProfilePictureObjectKey { get; set; } = string.Empty;
    public Guid? CommunityId { get; set; }
    public CommunityType? CommunityType { get; set; }
    public string? CommunityName { get; set; }
    public string? CommunityLogoObjectKey { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastInteractedAt { get; set; } = null;
    public List<MediaSnapshot> Medias { get; set; } = new List<MediaSnapshot>();
    public List<string> Topics { get; set; } = new List<string>();
    public List<string> Tags { get; set; } = new List<string>();
    public int CommentsCount { get; set; }
    public int ReactionsCount { get; set; }
    public int ViewsCount { get; set; }
    public int SavesCount { get; set; }
}

public sealed class MediaSnapshot
{
    public string ObjectKey { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
}

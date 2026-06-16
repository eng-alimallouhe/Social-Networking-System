using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.ContentManagement.Posts.Enums;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Posts.Entities;

public class Post : Entity, ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) to Many(ContentManagement)
    public Guid AuthorId { get; private set; }

    // Foreign Key: One(Community) to Many(ContentManagement) - Optional 
    public Guid? CommunityId { get; private set; }

    // General Fields
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public bool IsPinned { get; private set; }
    public PostType Type { get; private set; }
    public PostStatus? Status { get; private set; }
    public int EngagementScore { get; private set; } 


    // Soft Delete
    public bool IsActive { get; private set; }


    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastInteractedAt { get; private set; } = null;

    // Navigation Properties
    public ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
    public ICollection<PostTopic> PostTopics { get; set; } = new List<PostTopic>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<PostReaction> Reactions { get; set; } = new List<PostReaction>();
    public ICollection<PostView> Views { get; set; } = new List<PostView>();
    public ICollection<SavedPost> SavedPosts { get; set; } = new List<SavedPost>();

    private Post()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Post Create(Guid authorId, Guid? communityId, string title, string content, bool isPinned, PostType type, PostStatus? status, int engagementScore)
    {
        var entity = new Post();
        entity.AuthorId = authorId;
        entity.CommunityId = communityId;
        entity.Title = title;
        entity.Content = content;
        entity.IsPinned = isPinned;
        entity.Type = type;
        entity.Status = status;
        entity.EngagementScore = engagementScore;
        return entity;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }
}


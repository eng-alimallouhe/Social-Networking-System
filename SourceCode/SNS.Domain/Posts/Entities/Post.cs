using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Communities.Entities;
using SNS.Domain.Content.Enums;
using SNS.Domain.Posts.Bridges;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Content.Entities;

public class Post : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid AuthorId { get; set; }
    public Guid? CommunityId { get; set; }

    // General Fields
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public PostType Type { get; set; }
    public PostStatus Status { get; set; }



    // Soft Delete
    public bool IsActive { get; set; }


    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public Profile Author { get; set; } = null!;
    public Community? Community { get; set; }
    public ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<PostReaction> Reactions { get; set; } = new List<PostReaction>();
    public ICollection<PostView> Views { get; set; } = new List<PostView>();
    public ICollection<PostTopic> Topics { get; set; } = new List<PostTopic>();
    public ICollection<PostTag> Tags { get; set; } = new List<PostTag>();

    public Post()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
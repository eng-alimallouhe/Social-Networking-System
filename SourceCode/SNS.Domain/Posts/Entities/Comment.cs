using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Posts.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Content.Entities;

public class Comment : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid AuthorId { get; set; }
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }

    // General
    public string Content { get; set; } = string.Empty;


    //Soft Delete
    public bool IsActive { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public Post Post { get; set; } = null!;
    public Profile Author { get; set; } = null!;
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
    public ICollection<CommentMedia> Medias { get; set; } 
        = new List<CommentMedia>();

    public Comment()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Comments.Entities;

public class Comment : Entity, ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) to Many(Comments)
    public Guid AuthorId { get; private set; }

    // Foreign Key: One(Post) to Many(Comments)
    public Guid PostId { get; private set; }

    // Foreign Key: One(Comment) to Many(Comments) for replies (Nullable for top-level comments)
    public Guid? ParentCommentId { get; private set; }

    // General
    public string Content { get; private set; } = string.Empty;


    //Soft Delete
    public bool IsActive { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation Properties
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public ICollection<CommentReaction> Reactions { get; set; } = new List<CommentReaction>();
    public ICollection<CommentMedia> Medias { get; set; } 
        = new List<CommentMedia>();

    private Comment()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Comment Create(Guid authorId, Guid postId, Guid? parentCommentId, string content)
    {
        var entity = new Comment();
        entity.AuthorId = authorId;
        entity.PostId = postId;
        entity.ParentCommentId = parentCommentId;
        entity.Content = content;
        return entity;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }
}

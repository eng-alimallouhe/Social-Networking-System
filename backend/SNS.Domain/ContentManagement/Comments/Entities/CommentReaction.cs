using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Comments.Entities;

public class CommentReaction : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Comment) to Many(CommentReaction)
    public Guid CommentId { get; private set; }

    // Foreign Key: One(Profile) to Many(CommentReaction)
    public Guid ReactorId { get; private set; }

    // General
    public ReactionType Type { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }

    private CommentReaction()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static CommentReaction Create(Guid commentId, Guid reactorId, ReactionType type)
    {
        var entity = new CommentReaction();
        entity.CommentId = commentId;
        entity.ReactorId = reactorId;
        entity.Type = type;
        return entity;
    }

    public void UpdateType(ReactionType newType)
    {
        Type = newType;
    }
}

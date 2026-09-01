using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.ContentManagement.Shared.Enums;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Posts.Entities;

public class PostReaction : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Post) to Many(PostReaction)
    public Guid PostId { get; private set; }

    //Foreign Key: One(Profile) to Many(PostReaction)
    public Guid ReactorId { get; private set; }

    // General
    public ReactionType Type { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }

    private PostReaction()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static PostReaction Create(Guid postId, Guid reactorId, ReactionType type)
    {
        var entity = new PostReaction();
        entity.PostId = postId;
        entity.ReactorId = reactorId;
        entity.Type = type;
        return entity;
    }

    public void UpdateType(ReactionType newType)
    {
        Type = newType;
    }
}

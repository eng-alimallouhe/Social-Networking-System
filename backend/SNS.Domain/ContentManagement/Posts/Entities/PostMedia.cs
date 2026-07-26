using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Posts.Entities;

public class PostMedia : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Post) to Many(PostMedia)
    public Guid PostId { get; private set; }

    // General
    public string ObjectKey { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public MediaType Type { get; private set; }
    public int Order { get; private set; }

    private PostMedia()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static PostMedia Create(Guid postId, string objectKey, string mimeType, MediaType type, int order)
    {
        var entity = new PostMedia();
        entity.PostId = postId;
        entity.ObjectKey = objectKey;
        entity.MimeType = mimeType;
        entity.Type = type;
        entity.Order = order;
        return entity;
    }


    public void SetOrder(int order)
    {
        this.Order = order;
    }
}
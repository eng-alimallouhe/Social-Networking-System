using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Posts.Entities;

public class PostTag : IHardDeletable
{
    //Primary Key:
    public Guid Id { get; private set; }

    //the PostId with the TagId is unique, meaning a Post can only have one instance of a specific Tag, and a Tag can only be associated with a specific Post once.
    //Foreign Key: One(Post) To Many(PostTags)
    public Guid PostId { get; private set; }

    //Foreign Key: One(Tag) To Many(PostTags)
    public Guid TagId { get; private set; }
    public float? Confidence { get; private set; }

    private PostTag()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static PostTag Create(Guid postId, Guid tagId, float? confidence = null)
    {
        var entity = new PostTag();
        entity.PostId = postId;
        entity.TagId = tagId;
        entity.Confidence = confidence;
        return entity;
    }
}

using SNS.Domain.Preferences.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Posts.Entities;

public class PostTopic : IHardDeletable
{
    //Primary key:
    public Guid Id { get; private set; }

    //Foreign key: One(Post) to Many(PostTopic)
    public Guid PostId { get; private set; }

    //Foreign key: One(Topic) to Many(PostTopics)
    public Guid TopicId { get; private set; }
    public float? Confidence { get; private set; }

    public Topic Topic { get; private set; } = null!;
    public Post Post { get; private set; } = null!;

    private PostTopic()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static PostTopic Create(Guid postId, Guid topicId, float? confidence = null)
    {
        var entity = new PostTopic();
        entity.PostId = postId;
        entity.TopicId = topicId;
        entity.Confidence = confidence;

        return entity;
    }
}

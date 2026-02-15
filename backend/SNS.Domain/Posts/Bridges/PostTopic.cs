using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Content.Entities;
using SNS.Domain.Preferences.Entities;

namespace SNS.Domain.Posts.Bridges;

public class PostTopic : IHardDeletable
{
    public Guid Id { get; set; }

    public Guid PostId { get; set; }
    public Guid TopicId { get; set; }
    public float? Confidence { get; set; }

    // Navigation
    public Post Post { get; set; } = null!;
    public Topic Topic { get; set; } = null!;

    public PostTopic()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
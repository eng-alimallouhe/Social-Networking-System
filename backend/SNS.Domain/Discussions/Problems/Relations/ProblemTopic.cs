using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;

namespace SNS.Domain.Discussions.Problems.Relations;

public class ProblemTopic : IHardDeletable
{
    //Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Problem) ? Many(ProblemTopics)
    public Guid ProblemId { get; private set; }

    // Foreign Key: One(Topic) ? Many(ProblemTopics)
    public Guid TopicId { get; private set; }

    // General Properties
    public float? Confidence { get; private set; }

    public Topic Topic { get; set; } = null!;
    public Problem Problem { get; set; } = null!;

    public static ProblemTopic Create(Guid problemId, Guid topicId, float? confidence = null)
    {
        return new ProblemTopic
        {
            Id = Guid.NewGuid(),
            ProblemId = problemId,
            TopicId = topicId,
            Confidence = confidence
        };
    }
}

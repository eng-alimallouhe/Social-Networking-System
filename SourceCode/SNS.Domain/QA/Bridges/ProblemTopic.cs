
using SNS.Domain.Abstractions.Common;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.QA.Entities;

namespace SNS.Domain.QA.Bridges;


public class ProblemTopic : IHardDeletable
{
    // Foreign Key: One(Problem) → Many(ProblemTopics)
    public Guid ProblemId { get; set; }

    // Foreign Key: One(Topic) → Many(ProblemTopics)
    public Guid TopicId { get; set; }

    // General Properties
    public float? Confidence { get; set; }

    // Navigation Properties
    public Problem Problem { get; set; } = null!;
    public Topic Topic { get; set; } = null!;
}

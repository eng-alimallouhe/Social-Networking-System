using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class TopicInterest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Keys
    public Guid InterestId { get; private set; }
    public Guid TopicId { get; private set; }

    // Navigation

    private TopicInterest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static TopicInterest Create(Guid interestId, Guid topicId)
    {
        return new TopicInterest
        {
            InterestId = interestId,
            TopicId = topicId
        };
    }
}

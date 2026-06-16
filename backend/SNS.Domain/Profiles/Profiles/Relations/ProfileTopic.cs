using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Profiles.Profiles.Relations;
            
public class ProfileTopic : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Keys: One(Profile) ? Many(ProfileTopics)
    public Guid ProfileId { get; private set; }

    // Foreign Keys: One(Topic) ? Many(ProfileTopics)
    public Guid TopicId { get; private set; }

    public double Score { get; private set; }

    // Timestamp
    public DateTime LastUpdate { get; private set; }

    // Navigation

    private ProfileTopic()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        LastUpdate = DateTime.UtcNow;
    }

    public static ProfileTopic Create(Guid profileId, Guid topicId, double score)
    {
        return new ProfileTopic
        {
            ProfileId = profileId,
            TopicId = topicId,
            Score = score
        };
    }
}

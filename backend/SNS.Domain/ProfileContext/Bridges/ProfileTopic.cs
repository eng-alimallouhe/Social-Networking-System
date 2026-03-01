using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.ProfileContext.Bridges;
            
public class ProfileTopic : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProfileId { get; set; }
    public Guid TopicId { get; set; }

    public double Score { get; set; }

    // Timestamp
    public DateTime LastUpdate { get; set; }

    // Navigation

    public ProfileTopic()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        LastUpdate = DateTime.UtcNow;
    }
}

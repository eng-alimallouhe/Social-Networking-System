using SNS.Domain.Preferences.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.ProfileContext.Bridges;
            
public class ProfileTopic
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
    public Profile Profile { get; set; } = null!;
    public Topic Topic { get; set; } = null!;
}

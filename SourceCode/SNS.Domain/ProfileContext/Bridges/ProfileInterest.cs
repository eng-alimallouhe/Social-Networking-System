using SNS.Domain.Preferences.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.ProfileContext.Bridges;

public class ProfileInterest
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid InterestId { get; set; }
    public Guid ProfileId { get; set; }

    // Navigation
    public Interest Interest { get; set; } = null!;
    public Profile Profile { get; set; } = null!;
}

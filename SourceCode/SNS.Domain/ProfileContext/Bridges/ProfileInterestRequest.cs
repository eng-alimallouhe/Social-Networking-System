using SNS.Domain.Preferences.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.ProfileContext.Bridges;

public class ProfileInterestRequest
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProfileId { get; set; }
    public Guid InterestRequestId { get; set; }

    // Navigation
    public Profile Profile { get; set; } = null!;
    public InterestRequest InterestRequest { get; set; } = null!;
}

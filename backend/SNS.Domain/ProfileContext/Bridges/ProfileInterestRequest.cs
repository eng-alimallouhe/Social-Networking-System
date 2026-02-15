using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.ProfileContext.Bridges;

public class ProfileInterestRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid JoinerId { get; set; }
    public Guid InterestRequestId { get; set; }

    // Navigation
    public Profile Joiner { get; set; } = null!;
    public InterestRequest InterestRequest { get; set; } = null!;

    public ProfileInterestRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
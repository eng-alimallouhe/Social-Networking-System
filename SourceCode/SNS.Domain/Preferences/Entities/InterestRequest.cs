using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.ProfileContext.Bridges;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Preferences.Entities;

public class InterestRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string InterestName { get; set; } = default!;
    public string? InterestDescription { get; set; }

    public RequestStatus RequestStatus { get; set; }
    
    // Foreign Key
    public Guid? RequestedByProfileId { get; set; }

    // Navigation
    public Profile? RequestedByProfile { get; set; }
    public ICollection<ProfileInterestRequest> ProfileInterestRequests { get; set; } = new List<ProfileInterestRequest>();
}

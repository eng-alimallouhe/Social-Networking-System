using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Common.Helpers;
using SNS.Domain.ProfileContext.Bridges;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Preferences.Entities;

public class InterestRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public RequestStatus Status { get; set; }
    
    // Foreign Key
    public Guid? SubmitterId { get; set; }

    // Navigation
    public Profile? Submitter { get; set; }
    public ICollection<ProfileInterestRequest> ProfileInterestRequests { get; set; } = new List<ProfileInterestRequest>();

    public InterestRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = RequestStatus.Pending;
    }
}
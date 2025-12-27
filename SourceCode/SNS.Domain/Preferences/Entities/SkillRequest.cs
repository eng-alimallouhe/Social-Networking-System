using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Preferences.Enums;
using SNS.Domain.ProfileContext.Bridges;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Preferences.Entities;

public class SkillRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string SkillName { get; set; } = default!;

    // Foreign Key: One(Profile) To Many(SkillRequests)
    public Guid RequestedByProfileId { get; set; }

    public ProficiencyLevel Level { get; set; }
    public RequestStatus Status { get; set; }
    
    // Navigation
    public ICollection<ProfileSkillRequest> Requests { get; set; } = new List<ProfileSkillRequest>();
    public Profile RequestedByProfile { get; set; } = null!;

    public SkillRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = RequestStatus.Pending;
    }
}
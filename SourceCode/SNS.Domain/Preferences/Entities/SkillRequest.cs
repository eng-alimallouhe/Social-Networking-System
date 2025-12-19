using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Enums;
using SNS.Domain.Preferences.Enums;
using SNS.Domain.ProfileContext.Bridges;

namespace SNS.Domain.Preferences.Entities;

public class SkillRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string SkillName { get; set; } = default!;

    // Foreign Key: One(Profile) To Many(SkillRequests)
    public Guid RequestedByProfile { get; set; }

    public ProficiencyLevel RequestedLevel { get; set; }
    public RequestStatus Status { get; set; }
    
    // Navigation
    public ICollection<ProfileSkillRequest> Requests { get; set; } = new List<ProfileSkillRequest>();
}

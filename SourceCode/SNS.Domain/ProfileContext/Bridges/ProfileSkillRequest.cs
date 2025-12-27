using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Preferences.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.ProfileContext.Bridges;

public class ProfileSkillRequest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProfileId { get; set; }
    public Guid SkillRequestId { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    public ProficiencyLevel Level { get; set; }

    // Navigation
    public SkillRequest SkillRequest { get; set; } = null!;
    public Profile Profile { get; set; } = null!;

    public ProfileSkillRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
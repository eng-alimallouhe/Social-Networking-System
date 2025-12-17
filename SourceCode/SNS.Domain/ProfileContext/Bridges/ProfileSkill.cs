using SNS.Domain.Preferences.Entities;
using SNS.Domain.Preferences.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.ProfileContext.Bridges;

public class ProfileSkill
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProfileId { get; set; }
    public Guid SkillId { get; set; }

    public ProficiencyLevel ProficiencyLevel { get; set; }

    // Navigation
    public Profile Profile { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

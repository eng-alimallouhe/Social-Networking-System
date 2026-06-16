using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Preferences.Enums;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Profiles.Profiles.Relations;

public class ProfileSkill : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Keys: One(Profile) ? Many(ProfileSkills)
    public Guid ProfileId { get; private set; }

    // Foreign Keys: One(Skill) ? Many(ProfileSkills)
    public Guid SkillId { get; private set; }

    public ProficiencyLevel Level { get; private set; }

    // Navigation
    public Skill Skill { get; private set; } = null!;

    private ProfileSkill()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ProfileSkill Create(Guid profileId, Guid skillId, ProficiencyLevel level)
    {
        return new ProfileSkill
        {
            ProfileId = profileId,
            SkillId = skillId,
            Level = level
        };
    }
}

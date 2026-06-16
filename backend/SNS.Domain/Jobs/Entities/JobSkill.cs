using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Preferences.Entities;

namespace SNS.Domain.Jobs.Entities;


public class JobSkill : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Job) → Many(JobSkills)
    public Guid JobId { get; private set; }

    // Foreign Key: One(Skill) → Many(JobSkills)
    public Guid SkillId { get; private set; }

    private JobSkill()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static JobSkill Create(Guid jobId, Guid skillId)
    {
        return new JobSkill
        {
            JobId = jobId,
            SkillId = skillId
        };
    }
}

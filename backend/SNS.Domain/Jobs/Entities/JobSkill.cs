using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Preferences.Entities;

namespace SNS.Domain.Jobs.Entities;


public class JobSkill : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Job) → Many(JobSkills)
    public Guid JobId { get; set; }

    // Foreign Key: One(Skill) → Many(JobSkills)
    public Guid SkillId { get; set; }

    public JobSkill()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}ن
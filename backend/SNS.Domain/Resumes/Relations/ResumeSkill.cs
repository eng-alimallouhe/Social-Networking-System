using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Resumes.Enums;

namespace SNS.Domain.Resumes.Bridges;

public class ResumeSkill : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    public Guid ResumeId { get; private set; }

    public string SkillName { get; private set; } = string.Empty;
    public ResumeSkillLevel Level { get; private set; }

    // Navigation
    public Resume Resume { get; private set; } = null!;

    private ResumeSkill()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ResumeSkill Create(Guid resumeId, string skillName, ResumeSkillLevel level)
    {
        return new ResumeSkill
        {
            ResumeId = resumeId,
            SkillName = skillName,
            Level = level
        };
    }
}

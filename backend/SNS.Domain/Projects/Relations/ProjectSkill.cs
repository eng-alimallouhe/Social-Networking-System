using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Projects.Entities;

namespace SNS.Domain.Projects.Bridges;    

public class ProjectSkill : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Keys
    public Guid ProjectId { get; private set; }
    public Guid SkillId { get; private set; }

    // Navigation
    public Skill Skill { get; set; } = null!;


    private ProjectSkill()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ProjectSkill Create(Guid projectId, Guid skillId)
    {
        return new ProjectSkill
        {
            ProjectId = projectId,
            SkillId = skillId
        };
    }
}

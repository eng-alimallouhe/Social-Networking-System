using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Projects.Entities;

namespace SNS.Domain.Projects.Bridges;    

public class ProjectSkill : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProjectId { get; set; }
    public Guid SkillId { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public Skill Skill { get; set; } = null!;

    public ProjectSkill()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
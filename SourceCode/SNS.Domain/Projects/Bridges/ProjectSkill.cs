using SNS.Domain.Projects.Entities;
using SNS.Domain.Preferences.Entities;

namespace SNS.Domain.Projects.Bridges;    

public class ProjectSkill
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProjectId { get; set; }
    public Guid SkillId { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

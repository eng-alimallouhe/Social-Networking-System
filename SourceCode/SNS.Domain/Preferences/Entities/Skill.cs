using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class Skill : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(SkillsCategory) To Many(Skills)
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = default!;
    
    // Soft Delete
    public bool IsActive { get; set; }

    // Navigation
    public SkillsCategory Category { get; set; } = null!;

    public Skill()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsActive = true;
    }
}
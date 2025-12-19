using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Preferences.Entities;

public class Skill : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(SkillsCategory) To Many(Skills)
    public Guid CategoryId { get; set; }

    public string SkillName { get; set; } = default!;
    
    // Soft Delete
    public bool IsActive { get; set; } = true; 

    // Navigation
    public SkillsCategory Category { get; set; } = null!;
}

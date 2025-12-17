namespace SNS.Domain.Preferences.Entities;

public class Skill
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(SkillsCategory) To Many(Skills)
    public Guid CategoryId { get; set; }

    public string SkillName { get; set; } = default!;
    public bool IsActive { get; set; } = true; // Soft Delete

    // Navigation
    public SkillsCategory Category { get; set; } = null!;
}

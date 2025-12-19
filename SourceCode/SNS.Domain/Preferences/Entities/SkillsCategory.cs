using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Preferences.Entities;

public class SkillsCategory : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string CategoryName { get; set; } = default!;
    public string? CategoryDescription { get; set; }
    
    // Soft Delete
    public bool IsActive { get; set; } = true; 


    // Navigation
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}

using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class SkillsCategory : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    
    // Soft Delete
    public bool IsActive { get; set; } 

    // Navigation
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public SkillsCategory()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsActive = true;
    }
}
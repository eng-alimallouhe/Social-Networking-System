using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class SkillsCategory : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    
    // Soft Delete
    public bool IsActive { get; private set; } 

    // Navigation
    public ICollection<Skill> Skills { get; private set; } = new List<Skill>();

    private SkillsCategory()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsActive = true;
    }

    public static SkillsCategory Create(string name, string? description)
    {
        return new SkillsCategory
        {
            Name = name,
            Description = description
        };
    }

    public void SoftDelete()
    {
        if (IsActive)
        {
            IsActive = false;
        }
    }
}

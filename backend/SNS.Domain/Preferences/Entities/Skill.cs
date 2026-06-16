using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class Skill : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(SkillsCategory) To Many(Skills)
    public Guid CategoryId { get; private set; }

    public string Name { get; private set; } = default!;
    
    // Soft Delete
    public bool IsActive { get; private set; }

    // Navigation
    public SkillsCategory Category { get; private set; } = null!;

    private Skill()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsActive = true;
    }

    public static Skill Create(string name, Guid categoryId)
    {
        return new Skill
        {
            Name = name,
            CategoryId = categoryId
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

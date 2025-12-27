using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class InterestCategory : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    // Soft Delete
    public bool IsActive { get; set; } = true; 

    // Navigation
    public ICollection<Interest> Interests { get; set; } = new List<Interest>();

    public InterestCategory()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}
using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Preferences.Entities;

public class InterestCategory : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string CategoryName { get; set; } = default!;
    public string? CategoryDescription { get; set; }

    // Soft Delete
    public bool IsActive { get; set; } = true; 

    // Navigation
    public ICollection<Interest> Interests { get; set; } = new List<Interest>();
}

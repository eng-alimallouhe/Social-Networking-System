namespace SNS.Domain.Preferences.Entities;

public class InterestCategory
{
    // Primary Key
    public Guid Id { get; set; }

    public string CategoryName { get; set; } = default!;
    public string? CategoryDescription { get; set; }

    public bool IsActive { get; set; } = true; // Soft Delete

    // Navigation
    public ICollection<Interest> Interests { get; set; } = new List<Interest>();
}

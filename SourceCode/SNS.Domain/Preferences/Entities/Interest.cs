using SNS.Domain.ProfileContext.Bridges;

namespace SNS.Domain.Preferences.Entities;

public class Interest
{
    // Primary Key
    public Guid Id { get; set; }

    public string InterestName { get; set; } = default!;
    public string? InterestDescription { get; set; }

    public bool IsActive { get; set; } = true; // Soft Delete

    // Navigation
    public ICollection<ProfileInterest> ProfileInterests { get; set; } = new List<ProfileInterest>();
}

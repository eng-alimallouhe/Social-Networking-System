using SNS.Domain.Abstractions.Common;
using SNS.Domain.ProfileContext.Bridges;

namespace SNS.Domain.Preferences.Entities;

public class Interest : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string InterestName { get; set; } = default!;
    public string? InterestDescription { get; set; }

    // Soft Delete
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<ProfileInterest> ProfileInterests { get; set; } = new List<ProfileInterest>();
}

using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.ProfileContext.Bridges;

namespace SNS.Domain.Preferences.Entities;

public class Interest : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key:
    public Guid CategoryId { get; set; }

    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    // Soft Delete
    public bool IsActive { get; set; }

    // Navigation
    public ICollection<ProfileInterest> ProfileInterests { get; set; } = new List<ProfileInterest>();
    public InterestCategory Category { get; set; } = null!;
    public ICollection<TopicInterest>  TopicInterests { get; set; } 
        = new List<TopicInterest>();

    public Interest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsActive = true;
    }
}

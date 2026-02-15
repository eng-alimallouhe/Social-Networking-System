using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.SocialGraph.Bridges;

public class Block : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(Blocked Profiles)
    public Guid BlockerId { get; set; }

    // Foreign Key: One(Profile) → Many(Profiles that blocked him)
    public Guid BlockedId { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Profile Blocker { get; set; } = default!;
    public Profile Blocked { get; set; } = default!;

    public Block()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
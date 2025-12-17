namespace SNS.Domain.SocialGraph;

public class Block
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(Blocked Profiles)
    public Guid BlockerProfileId { get; set; }

    // Foreign Key: One(Profile) → Many(Profiles that blocked him)
    public Guid BlockedProfileId { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation
    public Profile Blocker { get; set; } = default!;
    public Profile Blocked { get; set; } = default!;
}

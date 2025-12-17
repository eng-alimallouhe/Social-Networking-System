namespace SNS.Domain.SocialGraph;

public class Follow
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(Follows as Followers)
    public Guid FollowerProfileId { get; set; }

    public Profile FollowerProfile { get; set; } = default!;

    // Foreign Key: One(Profile) → Many(Follows as Followings)
    public Guid FollowingProfileId { get; set; }

    public Profile FollowingProfile { get; set; } = default!;

    // Timestamp
    public DateTime CreatedAt { get; set; }
}
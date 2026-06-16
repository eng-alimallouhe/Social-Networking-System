using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Profiles.SocialGraph.Entities;

public class Follow : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) ? Many(Follows as Followers)
    public Guid FollowerId { get; private set; }


    // Foreign Key: One(Profile) ? Many(Follows as Followings)
    public Guid FollowingId { get; private set; }





    // Timestamp
    public DateTime CreatedAt { get; private set; }

    private Follow()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static Follow Create(Guid followerId, Guid followingId)
    {
        return new Follow
        {
            FollowerId = followerId,
            FollowingId = followingId
        };
    }
}

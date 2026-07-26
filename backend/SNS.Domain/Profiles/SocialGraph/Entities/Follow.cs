using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Helpers;
using SNS.Shared.Exceptions;

namespace SNS.Domain.Profiles.SocialGraph.Entities;

public class Follow : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) ? Many(Follows as Followers)
    public Guid FollowerId { get; private set; }


    // Foreign Key: One(Profile) ? Many(Follows as Followings)
    public Guid FollowingId { get; private set; }

    // Mute Control
    public bool IsMuted 
        => MutedUntil != null && 
            MutedUntil < DateTime.UtcNow;

    public DateTime? MutedAt { get; private set; } = null;
    public DateTime? MutedUntil { get; private set; } = null;

    // Timestamp
    public DateTime CreatedAt { get; private set; }


    // Navigation Properties
    public Profile Follower { get; set; } = null!; 
    public Profile Following { get; set; } = null!; 

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


    public void Mute(TimePeriod period)
    {
        var mutedUntil = period switch
        {
            TimePeriod.Year => DateTime.UtcNow.AddYears(1),
            TimePeriod.Month => DateTime.UtcNow.AddMonths(1),
            TimePeriod.ThreeMonth => DateTime.UtcNow.AddMonths(3),
            TimePeriod.Week => DateTime.UtcNow.AddDays(7),
            _ => DateTime.UtcNow.AddDays(1)
        };
        
        this.MutedAt = DateTime.UtcNow;
        this.MutedUntil = mutedUntil;
    }

    public void UnMute()
    {
        MutedUntil = null;
        MutedAt = null;
    }
}

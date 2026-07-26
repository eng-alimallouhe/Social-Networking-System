using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Profiles.SocialGraph.Entities;

public class Block : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) ? Many(Blocked Profiles)
    public Guid BlockerId { get; private set; }

    // Foreign Key: One(Profile) ? Many(Profiles that blocked him)
    //This is not unique
    //Cann't set BlockedId iqual to BlockerId 
    public Guid BlockedId { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Navigation
    public Profile Blocker { get; set; } = null!;
    public Profile Blocked { get; set; } = null!;


    private Block()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Block Create(Guid blockerId, Guid blockedId)
    {
        return new Block
        {
            BlockerId = blockerId,
            BlockedId = blockedId
        };
    }
}

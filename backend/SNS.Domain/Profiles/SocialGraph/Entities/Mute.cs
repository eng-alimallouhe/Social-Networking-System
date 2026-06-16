using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Profiles.SocialGraph.Entities;

public class Mute : Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public Guid MutedId { get; private set; }
    public Guid MuterId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    //Navigation properties:
    public Profile Muted { get; private set; } = null!;
    public Profile Muter { get; private set; } = null!;

    private Mute()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static Mute Create(Guid mutedId, Guid muterId)
    {
        return new Mute
        {
            MutedId = mutedId,
            MuterId = muterId
        };
    }
}

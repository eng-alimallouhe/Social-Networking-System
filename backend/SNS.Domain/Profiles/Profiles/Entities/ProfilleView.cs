using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Profiles.Profiles.Entities;

public class ProfileView : Entity, ISoftDeletable
{
    public Guid Id { get; private set; }
    public Guid ViewedId { get; private set; }
    public Guid ViewerId { get; private set; }
    public DateTime ViewedAt { get; private set; }

    //Soft Delete:
    public bool IsActive { get; private set; }

    public Profile Viewer { get; set; } = null!;
    public Profile Viewed { get; set; } = null!;


    private ProfileView()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        ViewedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }

    public static ProfileView Create(Guid viewedId, Guid viewerId)
    {
        return new ProfileView
        {
            ViewedId = viewedId,
            ViewerId = viewerId
        };
    }
}

using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.SocialGraph.Bridges;

public class ProfileView : IHardDeletable
{
    public Guid Id { get; set; }
    public Guid ViewedId { get; set; }
    public Guid ViewerId { get; set; }
    public DateTime ViewedAt { get; set; }

    public Profile Viewer { get; set; } = null!;
    public Profile Viewed { get; set; } = null!;

    public ProfileView()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        ViewedAt = DateTime.UtcNow;
    }
}
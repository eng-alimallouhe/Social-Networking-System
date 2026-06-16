using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Discussions.Problems.Relations;

public class ProblemView : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Problem) ? Many(ProblemViews)
    public Guid ProblemId { get; private set; }

    // Foreign Key: One(Profile) ? Many(ProblemViews)
    public Guid ViewerId { get; private set; }

    // Timestamp
    public DateTime ViewedAt { get; private set; }

    // Optional Info
    public DeviceType? DeviceType { get; private set; }
    public string? IpHash { get; private set; }
    public string? Country { get; private set; }

    //Soft Delet:
    public bool IsActive { get; private set; }

    public ProblemView()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        ViewedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static ProblemView Create(Guid problemId, Guid viewerId, DeviceType? deviceType = null, string? ipHash = null, string? country = null)
    {
        return new ProblemView
        {
            ProblemId = problemId,
            ViewerId = viewerId,
            DeviceType = deviceType,
            IpHash = ipHash,
            Country = country
        };
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }
}

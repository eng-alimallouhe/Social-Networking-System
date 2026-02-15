using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Security.Enums;

namespace SNS.Domain.Security.Entities;

public class UserArchive : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; } = Guid.NewGuid();


    // Foreign Key: One(User) To Many(Archives)
    public Guid UserId { get; set; }

    public ActionType Type { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime TimeStamp { get; set; }

    public string? Reason { get; set; }

    // Foreign Key: One(User) To Many(ActionPerformed)
    public Guid PerformedBy { get; set; }

    public UserArchive()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        TimeStamp = DateTime.UtcNow;
    }
}
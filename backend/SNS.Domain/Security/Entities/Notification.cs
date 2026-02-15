using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Security.Enums;

namespace SNS.Domain.Security.Entities;

public class Notification : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(User) To Many(Notifications)
    public Guid UserId { get; set; }

    public Guid? ActorProfileId { get; set; }
    public NotificationSource Source { get; set; }
    public NotificationType Type { get; set; }
    public Guid TargetId { get; set; }

    public bool IsRead { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }

    public Notification()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsRead = false;
    }
}
using SNS.Domain.Identity.Notifications.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;
using SNS.Shared.Exceptions;

namespace SNS.Domain.Identity.Notifications.Entities;

public class Notification : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }


    // Foreign Key: One(User) To Many(Notifications) - Required (Every notification must be associated with a user)
    public Guid UserId { get; private set; }

    // Foreign Key: One(Profile) To Many(Notifications) - Optional (ActorProfileId can be null if the notification is system-generated without a specific actor)
    public Guid? ActorProfileId { get; private set; }
    
    public NotificationSource Source { get; private set; }
    public NotificationType Type { get; private set; }
    public Guid TargetId { get; private set; }
    public string RedirectUrl { get; private set; } = string.Empty;

    public bool IsRead { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }

    private Notification()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsRead = false;
    }

    public static Notification Create(
        Guid userId, 
        Guid? actorProfileId, 
        NotificationSource source, 
        NotificationType type, 
        Guid targetId,
        string redirectUrl)
    {
        var entity = new Notification();
        entity.UserId = userId;
        entity.ActorProfileId = actorProfileId;
        entity.Source = source;
        entity.Type = type;
        entity.TargetId = targetId;
        entity.RedirectUrl = redirectUrl;
        return entity;
    }

    public void MarkAsRead()
    {
        if (this.IsRead)
        {
            throw new DomainException("notification is already readed");
        }
        this.IsRead = true;
    }
}

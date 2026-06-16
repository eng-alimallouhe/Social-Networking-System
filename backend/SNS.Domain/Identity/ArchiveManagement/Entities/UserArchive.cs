using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;
using System.Text.Json;

namespace SNS.Domain.Identity.ArchiveManagement.Entities;

public class UserArchive : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; } = Guid.NewGuid();


    // Foreign Key: One(User) To Many(Archives)
    public Guid TargetId { get; private set; }
    public Guid? PerformedById { get; private set; }


    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime TimeStamp { get; private set; }

    public ActionType Type { get; private set; }
    public string? Reason { get; private set; }
    public string? Parameters { get; private set; }

    private UserArchive()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        TimeStamp = DateTime.UtcNow;
    }

    public static UserArchive Create(
        Guid targetId, 
        Guid? performedById, 
        ActionType type, 
        string? reason,
        Dictionary<ReplacementKey,
            string>? parameters = null)
    {
        var entity = new UserArchive();
        entity.TargetId = targetId;
        entity.PerformedById = performedById;
        entity.Type = type;
        entity.Reason = reason;
        entity.Parameters = parameters != null ? JsonSerializer.Serialize(parameters) : null;
        return entity;
    }
}

using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Communities.Entities;

public class CommunityAuditLog : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Community) ? Many(Logs)
    public Guid CommunityId { get; private set; }

    // Foreign Key: One(Profile) ? Many(AuditLogs) == Optional
    public Guid? ActorId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    // Timestamp
    public DateTime PerformedAt { get; private set; }

    private CommunityAuditLog()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        PerformedAt = DateTime.UtcNow;
    }

    public static CommunityAuditLog Create(Guid communityId, Guid? actorId, string action)
    {
        var entity = new CommunityAuditLog();
        entity.CommunityId = communityId;
        entity.ActorId = actorId;
        entity.Action = action;
        return entity;
    }
}

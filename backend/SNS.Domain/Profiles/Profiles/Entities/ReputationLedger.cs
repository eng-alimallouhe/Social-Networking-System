using SNS.Domain.Profiles.Profiles.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;

namespace SNS.Domain.Profiles.Profiles.Entities;

public class ReputationLedger : Entity, IHardDeletable
{
    //Primary Key: 
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) ? Many(ReputationLdger)
    // This is not Uninque
    public Guid ProfileId { get; private set; }

    public ReputationActionType ActionType { get; private set; }
    public int PointsDelta { get; private set; }

    //Can by Null if the action is not related to a specific entity, e.g., a general reputation boost or penalty.
    public Guid? SourceEntityId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ReputationLedger()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public static ReputationLedger Create(Guid profileId, ReputationActionType actionType, int pointsDelta, Guid? sourceEntityId)
    {
        var entity = new ReputationLedger();
        entity.ProfileId = profileId;
        entity.ActionType = actionType;
        entity.PointsDelta = pointsDelta;
        entity.SourceEntityId = sourceEntityId;
        return entity;
    }
}

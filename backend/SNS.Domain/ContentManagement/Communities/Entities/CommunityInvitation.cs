using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Communities.Entities;

public class CommunityInvitation : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    //The CommunitId with the inviteeId should be unique to prevent duplicate invitations

    // Foreign Key: One(Community) ? Many(Invitations)
    public Guid CommunityId { get; private set; }

    // Foreign Key: One(Profile) ? Many(SentInvitations)
    public Guid InviterId { get; private set; }

    // Foreign Key: One(Profile) ? Many(ReceivedInvitations)
    public Guid InviteeId { get; private set; }

    // Timestamp
    public DateTime SentAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }

    public InvitationStatus Status { get; private set; }

    private CommunityInvitation()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = InvitationStatus.Pending;
        SentAt = DateTime.UtcNow;
    }

    public static CommunityInvitation Create(Guid communityId, Guid inviterId, Guid inviteeId)
    {
        var entity = new CommunityInvitation();
        entity.CommunityId = communityId;
        entity.InviterId = inviterId;
        entity.InviteeId = inviteeId;
        return entity;
    }
}


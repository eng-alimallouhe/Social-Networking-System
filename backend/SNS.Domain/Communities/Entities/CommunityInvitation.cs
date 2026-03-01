using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Communities.Enums;

namespace SNS.Domain.Communities.Entities;

public class CommunityInvitation : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Community) ? Many(Invitations)
    public Guid CommunityId { get; set; }

    // Foreign Key: One(Profile) ? Many(SentInvitations)
    public Guid InviterId { get; set; }

    // Foreign Key: One(Profile) ? Many(ReceivedInvitations)
    public Guid InviteeId { get; set; }

    // Timestamp
    public DateTime SentAt { get; set; }
    public DateTime? RespondedAt { get; set; }

    public InvitationStatus Status { get; set; }

    public CommunityInvitation()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = InvitationStatus.Pending;
        SentAt = DateTime.UtcNow;
    }
}


using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Communities.Entities;

public class CommunityJoinRequest : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    //CommunityId with SubmitterId should be unique to prevent duplicate join requests from the same profile to the same community
    // Foreign Key: One(Community) ? Many(JoinRequests)
    public Guid CommunityId { get; private set; }

    // Foreign Key: One(Profile) ? Many(JoinRequests)
    public Guid SubmitterId { get; private set; }

    public JoinRequestStatus Status { get; private set; }
    public string Notes { get; private set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }


    private CommunityJoinRequest()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = JoinRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static CommunityJoinRequest Create(Guid communityId, Guid submitterId, string notes)
    {
        var entity = new CommunityJoinRequest();
        entity.CommunityId = communityId;
        entity.SubmitterId = submitterId;
        entity.Notes = notes;
        return entity;
    }
}


using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Communities.Entities;

public class CommunitySettings : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Community) ? One(Settings)
    public Guid CommunityId { get; private set; }

    public bool AllowPostWithoutApproval { get; private set; }
    public bool AllowInvitationsByMembers { get; private set; }
    public bool AllowComments { get; private set; }
    public bool AllowMediaUpload { get; private set; }

    private CommunitySettings()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static CommunitySettings Create(Guid communityId, bool allowPostWithoutApproval, bool allowInvitationsByMembers, bool allowComments, bool allowMediaUpload)
    {
        var entity = new CommunitySettings();
        entity.CommunityId = communityId;
        entity.AllowPostWithoutApproval = allowPostWithoutApproval;
        entity.AllowInvitationsByMembers = allowInvitationsByMembers;
        entity.AllowComments = allowComments;
        entity.AllowMediaUpload = allowMediaUpload;
        return entity;
    }
}


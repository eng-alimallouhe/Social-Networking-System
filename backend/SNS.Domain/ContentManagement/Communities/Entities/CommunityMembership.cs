using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Communities.Entities;

public class CommunityMembership : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    //MemeberId with CommunityId should be unique to prevent duplicate memberships for the same user in the same community
    // Foreign Key: One(Profile) ? Many(Memberships)
    public Guid MemberId { get; private set; }

    // Foreign Key: One(Community) ? Many(Memberships)
    public Guid CommunityId { get; private set; }

    public CommunityMembershipStatus Status { get; private set; }
    public CommunityRole Role { get; private set; }
    public DateTime JoinedDate { get; private set; } = DateTime.UtcNow;


    private CommunityMembership()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = CommunityMembershipStatus.Active;
        Role = CommunityRole.Member;
        JoinedDate = DateTime.UtcNow;
    }

    public static CommunityMembership Create(Guid memberId, Guid communityId)
    {
        var entity = new CommunityMembership();
        entity.MemberId = memberId;
        entity.CommunityId = communityId;
        return entity;
    }
}

using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.ContentManagement.Communities.Entities;

public class CommunityMembership : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // MemberId with CommunityId should be unique to prevent duplicate memberships for the same user in the same community
    // Foreign Key: One(Profile) ? Many(Memberships)
    public Guid MemberId { get; private set; }

    // Foreign Key: One(Community) ? Many(Memberships)
    public Guid CommunityId { get; private set; }

    public CommunityMembershipStatus Status { get; private set; }
    public CommunityRole Role { get; private set; }
    public DateTime JoinedDate { get; private set; } = DateTime.UtcNow;

    // Navigation Properties
    public Community Community { get; set; } = null!;
    public Profile Member { get; set; } = null!;

    private CommunityMembership()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = CommunityMembershipStatus.Active;
        Role = CommunityRole.Member;
        JoinedDate = DateTime.UtcNow;
    }

    public static CommunityMembership Create(Guid memberId, Guid communityId, CommunityRole role = CommunityRole.Member, CommunityMembershipStatus status = CommunityMembershipStatus.Active)
    {
        var entity = new CommunityMembership();
        entity.MemberId = memberId;
        entity.CommunityId = communityId;
        entity.Role = role;
        entity.Status = status;
        return entity;
    }

    public void ChangeRole(CommunityRole newRole)
    {
        Role = newRole;
    }

    public void UpdateStatus(CommunityMembershipStatus newStatus)
    {
        Status = newStatus;
    }
}

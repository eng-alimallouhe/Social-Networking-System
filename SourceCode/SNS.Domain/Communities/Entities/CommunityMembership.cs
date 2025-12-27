using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Communities.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Communities.Entities;

public class CommunityMembership : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Profile) → Many(Memberships)
    public Guid ProfileId { get; set; }

    // Foreign Key: One(Community) → Many(Memberships)
    public Guid CommunityId { get; set; }

    public CommunityMembershipStatus Status { get; set; }
    public CommunityRole Role { get; set; }
    public DateTime JoinedDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties (Required)
    public Profile Profile { get; set; } = null!;
    public Community Community { get; set; } = null!;


    public CommunityMembership()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        Status = CommunityMembershipStatus.Active;
        Role = CommunityRole.Member;
        JoinedDate = DateTime.UtcNow;
    }
}
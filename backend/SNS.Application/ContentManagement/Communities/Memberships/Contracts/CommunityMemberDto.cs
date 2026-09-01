using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Communities.Memberships.Contracts;

/// <summary>
/// Represents a member item in a community's member list.
/// </summary>
/// <param name="MembershipId">The unique membership record identifier.</param>
/// <param name="Member">The profile snapshot of the member.</param>
/// <param name="Role">The role assigned to the member in the community.</param>
/// <param name="Status">The membership status.</param>
/// <param name="JoinedDate">The timestamp when the member joined.</param>
public sealed record CommunityMemberDto(
    Guid MembershipId,
    ProfileSnapshotDto Member,
    CommunityRole Role,
    CommunityMembershipStatus Status,
    DateTime JoinedDate
);

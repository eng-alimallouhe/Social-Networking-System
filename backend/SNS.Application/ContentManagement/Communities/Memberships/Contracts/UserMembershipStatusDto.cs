using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Communities.Memberships.Contracts;

/// <summary>
/// Represents the membership and join request status of the current user for a community.
/// </summary>
/// <param name="IsMember">Indicates whether the current user is an active member.</param>
/// <param name="Role">The community role of the user, if a member.</param>
/// <param name="Status">The membership status of the user, if a record exists.</param>
/// <param name="HasPendingRequest">Indicates whether the user has a pending join request.</param>
public sealed record UserMembershipStatusDto(
    bool IsMember,
    CommunityRole? Role,
    CommunityMembershipStatus? Status,
    bool HasPendingRequest
);

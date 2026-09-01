using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Communities.Communities.Contracts;

/// <summary>
/// Represents full detailed community profile information including membership state and counters.
/// </summary>
/// <param name="Id">The unique identifier of the community.</param>
/// <param name="Name">The unique name of the community.</param>
/// <param name="Description">The community description.</param>
/// <param name="RulesText">The summary text or guidelines of community rules.</param>
/// <param name="Policy">The moderation policy of the community.</param>
/// <param name="Type">The privacy/visibility type of the community.</param>
/// <param name="Status">The active/archived status of the community.</param>
/// <param name="LogoUrl">Optional resolved temporary logo URL.</param>
/// <param name="MembersCount">Total active members count.</param>
/// <param name="PostsCount">Total active posts count.</param>
/// <param name="CreatedAt">The creation timestamp.</param>
/// <param name="UpdatedAt">The last update timestamp.</param>
/// <param name="Owner">The profile snapshot of the community owner.</param>
/// <param name="IsMember">Indicates whether the current authenticated user is an active member.</param>
/// <param name="CurrentUserRole">The role of the current user in the community, if a member.</param>
/// <param name="HasPendingJoinRequest">Indicates whether the current user has a pending join request.</param>
public sealed record CommunityDetailsDto(
    Guid Id,
    string Name,
    string Description,
    string RulesText,
    ModerationPolicy Policy,
    CommunityType Type,
    CommunityStatus Status,
    string? LogoUrl,
    int MembersCount,
    int PostsCount,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    ProfileSnapshotDto Owner,
    bool IsMember,
    CommunityRole? CurrentUserRole,
    bool HasPendingJoinRequest
);

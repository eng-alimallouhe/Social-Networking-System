using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.ContentManagement.Communities.Memberships.Queries.IsMember;

/// <summary>
/// Represents a query to check if a specific profile is an active member of a community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="ProfileId">The unique identifier of the profile.</param>
public sealed record IsMemberQuery(
    Guid CommunityId,
    Guid ProfileId
) : IQuery<bool>;

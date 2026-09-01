using SNS.Application.ContentManagement.Communities.Memberships.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.ContentManagement.Communities.Memberships.Queries.GetMyMembership;

/// <summary>
/// Represents a query to retrieve the current user's membership and request status in a community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
public sealed record GetMyMembershipQuery(Guid CommunityId) : IQuery<UserMembershipStatusDto>;

using SNS.Application.ContentManagement.Communities.Memberships.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.ContentManagement.Communities.Memberships.Queries.GetMembershipRequests;

/// <summary>
/// Represents a query to retrieve paginated pending join requests for a private community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="Page">The page number for pagination.</param>
/// <param name="PageSize">The number of items per page.</param>
public sealed record GetMembershipRequestsQuery(
    Guid CommunityId,
    int Page = 1,
    int PageSize = 20
) : IQuery<Paged<MembershipRequestDto>>;

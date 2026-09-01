using SNS.Application.ContentManagement.Communities.Memberships.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Communities.Memberships.Queries.GetCommunityMembers;

/// <summary>
/// Represents a query to retrieve paginated members of a community, optionally filtered by role.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="Role">Optional filter by community role.</param>
/// <param name="Page">The page number for pagination.</param>
/// <param name="PageSize">The number of items per page.</param>
public sealed record GetCommunityMembersQuery(
    Guid CommunityId,
    CommunityRole? Role = null,
    int Page = 1,
    int PageSize = 20
) : IQuery<Paged<CommunityMemberDto>>;

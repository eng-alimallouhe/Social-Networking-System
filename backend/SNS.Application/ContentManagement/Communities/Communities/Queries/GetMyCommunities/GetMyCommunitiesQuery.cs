using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.ContentManagement.Communities.Communities.Queries.GetMyCommunities;

/// <summary>
/// Represents a query to retrieve paginated communities where the current user is owner or an active member.
/// </summary>
/// <param name="Page">The page number for pagination.</param>
/// <param name="PageSize">The number of items per page.</param>
public sealed record GetMyCommunitiesQuery(
    int Page = 1,
    int PageSize = 10
) : IQuery<Paged<CommunitySummaryDto>>;

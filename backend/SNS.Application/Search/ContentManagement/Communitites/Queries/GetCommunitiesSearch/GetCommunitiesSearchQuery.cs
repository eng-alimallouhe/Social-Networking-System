using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.Search.ContentManagement.Communitites.Queries.GetCommunitiesSearch;

/// <summary>
/// Represents a search query to search for community documents in the search index using specified filter parameters.
/// </summary>
/// <param name="SearchTerm">Optional keyword to search within community name and description.</param>
/// <param name="Type">Optional community type filter.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of community records to return per page.</param>
public sealed record GetCommunitiesSearchQuery(
    string? SearchTerm = null,
    CommunityType? Type = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<SearchResult<CommunitySummaryDto>>;

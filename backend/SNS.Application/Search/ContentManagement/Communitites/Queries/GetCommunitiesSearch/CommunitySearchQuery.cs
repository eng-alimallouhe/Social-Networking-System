using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.Search.ContentManagement.Communitites.Queries;

/// <summary>
/// Represents filter parameters to query and search community documents in the search index.
/// </summary>
/// <param name="SearchTerm">Optional keyword to search within community name and description.</param>
/// <param name="Type">Optional community type filter.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of community records to return per page.</param>
public sealed record CommunitySearchQuery(
    string? SearchTerm = null,
    CommunityType? Type = null, 
    int Page = 1,
    int PageSize = 10);


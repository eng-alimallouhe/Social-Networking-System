using SNS.Application.Search.Queries.GetGlobalSearch;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.Queries.GlobalSearch;

/// <summary>
/// Represents a global search query to perform parallel searches across multiple entity categories.
/// </summary>
/// <param name="SearchTerm">The search keyword or term to query across categories.</param>
/// <param name="TopResultsPerCategory">The maximum number of top search results to return per category.</param>
public sealed record GetGlobalSearchQuery(
    string SearchTerm,
    int TopResultsPerCategory = 4 
) : IQuery<GlobalSearchResultDto>;


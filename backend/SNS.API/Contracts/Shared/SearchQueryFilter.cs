namespace SNS.API.Contracts.Shared;

/// <summary>
/// Represents general search and pagination query parameters submitted by the client.
/// </summary>
/// <param name="SearchTerm">Optional search term keyword for filtering items.</param>
/// <param name="CurrentPage">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of items to return per page.</param>
public sealed record SearchQueryFilter(
    string? SearchTerm,
    int CurrentPage = 1,
    int PageSize = 10
);

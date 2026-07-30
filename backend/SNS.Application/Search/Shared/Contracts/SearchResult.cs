namespace SNS.Application.Search.Shared.Contracts;

/// <summary>
/// Encapsulates search results returned from search index queries containing matched hit documents and total result count.
/// </summary>
/// <typeparam name="TDocument">The document model type contained in search hits.</typeparam>
public sealed class SearchResult<TDocument>
{
    /// <summary>
    /// Gets or sets the list of search hit items matching the query.
    /// </summary>
    public List<SearchHit<TDocument>> Hits { get; set; } = [];

    /// <summary>
    /// Gets or sets the total number of matching search hits across all pages.
    /// </summary>
    public long Total { get; set; }
}

/// <summary>
/// Represents an individual search result hit containing the matched document model and relevance score.
/// </summary>
/// <typeparam name="TDocument">The type of document model stored in the hit.</typeparam>
/// <param name="Document">The document model entity retrieved from search index.</param>
/// <param name="Score">The relevance score calculated by the search engine.</param>
public sealed record SearchHit<TDocument>(
    TDocument Document,
    double Score
);
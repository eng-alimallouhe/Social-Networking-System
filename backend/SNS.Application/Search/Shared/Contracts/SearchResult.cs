namespace SNS.Application.Search.Shared.Contracts;

public sealed class SearchResult<TDocument>
{
    public List<SearchHit<TDocument>> Hits { get; set; } = [];

    public long Total { get; set; }
}


public sealed record SearchHit<TDocument>(
    TDocument Document,
    double Score
);
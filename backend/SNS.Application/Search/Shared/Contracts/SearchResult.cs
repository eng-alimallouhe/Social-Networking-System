namespace SNS.Application.Search.Shared.Contracts;

public class SearchResult<TDocument>
{
    public List<TDocument> Documents { get; set; } = new List<TDocument>();
    public long Total { get; set; }
}

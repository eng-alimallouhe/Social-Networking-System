using Elastic.Clients.Elasticsearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Infrastructure.Search.Abstractions;
using SNS.Shared.StatusCodes;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Infrastructure.Search.Services;

public class ElasticDocumentService<TDocument> : IElasticDocumentService<TDocument> where TDocument : class
{
    private readonly ElasticsearchClient _client;

    public ElasticDocumentService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<AppResult> IndexDocumentAsync(string indexName, TDocument document, CancellationToken cancellationToken = default)
    {
        var response = await _client.IndexAsync(document, indexName, cancellationToken);

        return response.IsValidResponse
            ? AppResult.Success(OperationStatusCode.Success)
            : AppResult.Failure(OperationStatusCode.Failure);
    }

    public async Task<AppResult> BulkIndexDocumentAsync(string indexName, IEnumerable<TDocument> documents, CancellationToken cancellationToken = default)
    {
        var response = await _client.BulkAsync(b => b
            .Index(indexName)
            .IndexMany(documents), cancellationToken);

        return response.IsValidResponse && !response.Errors
            ? AppResult.Success(OperationStatusCode.Success)
            : AppResult.Failure(OperationStatusCode.Failure);
    }

    public async Task<AppResult> UpdateAsync(string indexName, string documentId, TDocument document, CancellationToken cancellationToken = default)
    {
        // In v8, we pass the generic types <TDocument, TDocument> to define the partial update payload
        var response = await _client.UpdateAsync<TDocument, TDocument>(indexName, documentId, u => u
            .Doc(document), cancellationToken);

        return response.IsValidResponse
            ? AppResult.Success(OperationStatusCode.Success)
            : AppResult.Failure(OperationStatusCode.Failure);
    }

    public async Task<AppResult> UpsertAsync(string indexName, string documentId, TDocument document, CancellationToken cancellationToken = default)
    {
        // Upsert combines Update and Index: If it exists, update it. If not, create it.
        var response = await _client.UpdateAsync<TDocument, TDocument>(indexName, documentId, u => u
            .Doc(document)
            .DocAsUpsert(true), cancellationToken); // The magic flag!

        return response.IsValidResponse
            ? AppResult.Success(OperationStatusCode.Success)
            : AppResult.Failure(OperationStatusCode.Failure);
    }

    public async Task<AppResult> DeleteAsync(string indexName, string documentId, CancellationToken cancellationToken = default)
    {
        var response = await _client.DeleteAsync<TDocument>(documentId, d => d.Index(indexName), cancellationToken);

        return response.IsValidResponse
            ? AppResult.Success(OperationStatusCode.Success)
            : AppResult.Failure(OperationStatusCode.Failure);
    }

    public async Task<AppResult> DeleteDocumentByIdAsync(string indexName, string documentId, CancellationToken cancellationToken = default)
    {
        // This is identical to DeleteAsync. Feel free to remove one from the interface!
        return await DeleteAsync(indexName, documentId, cancellationToken);
    }

    public async Task<SearchResult<TDocument>> SearchAsync(string indexName, Action<SearchRequestDescriptor<TDocument>> descriptor, CancellationToken cancellationToken = default)
    {
        var response = await _client.SearchAsync<TDocument>(s => {
            s.Indices(indexName);
            descriptor(s);
        }, cancellationToken);

        if (!response.IsValidResponse)
        {
            // You might want to return SearchResult<TDocument>.Failure() depending on your DTO setup
            throw new Exception($"Elasticsearch query failed: {response.DebugInformation}");
        }

        // Map the Elastic response to your custom SearchResult DTO
        return new SearchResult<TDocument>
        {
            Total = response.Total,

            Hits = response.Hits
            .Where(h => h.Source != null)
            .Select(h => new SearchHit<TDocument>(
                h.Source!,
                h.Score ?? 0))
            .ToList()
        };
    }

    public async Task<int> CountAsync(string indexName, Action<SearchRequestDescriptor<TDocument>> descriptor, CancellationToken cancellationToken = default)
    {
        // Since the interface passes a SearchDescriptor, we run a search but set Size(0).
        // This calculates the total matching documents instantly without returning any data payloads.
        var response = await _client.SearchAsync<TDocument>(s => {
            s.Indices(indexName);
            s.Size(0); // Do not return actual documents
            descriptor(s);
        }, cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new Exception($"Elasticsearch count query failed: {response.DebugInformation}");
        }

        return (int)response.Total;
    }

    public async Task<AppResult> DeleteByQueryAsync(
        string indexName,
        Action<DeleteByQueryRequestDescriptor<TDocument>> descriptor, CancellationToken cancellationToken = default)
    {
        var result = await _client.DeleteByQueryAsync(
            indexName,
            descriptor, 
            cancellationToken);

        return result.IsValidResponse
            ? AppResult.Success(OperationStatusCode.Success)
            : AppResult.Failure(OperationStatusCode.Failure);
    }
}

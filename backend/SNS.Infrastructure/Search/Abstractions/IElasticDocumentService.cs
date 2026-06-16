using Elastic.Clients.Elasticsearch;
using SNS.Application.Search.Shared.Contracts;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Infrastructure.Search.Abstractions;

public interface IElasticDocumentService<TDocument> where TDocument : class
{
    Task<AppResult> IndexDocumentAsync(string indexName, TDocument document, CancellationToken cancellationToken = default);
    Task<AppResult> BulkIndexDocumentAsync(string indexName, IEnumerable<TDocument> documents, CancellationToken cancellationToken = default);
    Task<AppResult> UpdateAsync(string indexName, string documentId, TDocument document, CancellationToken cancellationToken = default);
    Task<AppResult> DeleteAsync(string indexName, string documentId, CancellationToken cancellationToken = default);
    Task<SearchResult<TDocument>> SearchAsync(string indexName, Action<SearchRequestDescriptor<TDocument>> descriptor, CancellationToken cancellationToken = default);
    Task<int> CountAsync(string indexName, Action<SearchRequestDescriptor<TDocument>> descriptor, CancellationToken cancellationToken = default);
    Task<AppResult> UpsertAsync(string indexName, string documentId, TDocument document, CancellationToken cancellationToken = default);
    Task<AppResult> DeleteDocumentByIdAsync(string indexName, string documentId, CancellationToken cancellationToken = default);
    Task<AppResult> DeleteByQueryAsync(
        string indexName,
        Action<DeleteByQueryRequestDescriptor<TDocument>> descriptor, CancellationToken cancellationToken = default);
}

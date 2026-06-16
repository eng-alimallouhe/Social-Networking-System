namespace SNS.Application.Search.Shared.Abstractions;

public interface IElasticIndexManager
{
    Task CreateIndexAsync(string indexName, object mappingObject, int numberOfShareds = 1, int numberOfReplicas = 1, CancellationToken cancellationToken = default);
    Task DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);
    Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default);
    Task ReindexAsync(string sourceIndexName, string targetIndexName, CancellationToken cancellationToken = default);
    Task UpdateMappingAsync(string indexName, object newMappingObject, CancellationToken cancellationToken = default);
}

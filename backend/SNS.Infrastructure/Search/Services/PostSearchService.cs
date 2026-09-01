using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Queries.GetPostsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using SNS.Infrastructure.Search.Abstractions;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Infrastructure.Search.Services;

public class PostSearchService : IPostSearchService
{
    private readonly IElasticDocumentService<PostDocument> _elasticBaseService;
    private readonly string _indexName = "sns_posts";

    public PostSearchService(IElasticDocumentService<PostDocument> elasticBaseService)
    {
        _elasticBaseService = elasticBaseService;
    }

    public async Task<SearchResult<PostDocument>> SearchAsync(GetPostsSearchQuery query, CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Fields = new[] { "title^3.0", "content", "tags", "topics" },
                Query = query.SearchTerm,
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        if (query.MinCreatedAt.HasValue || query.MaxCreatedAt.HasValue)
        {
            filterQueries.Add(new DateRangeQuery
            {
                Field = "createdAt",
                Gte = query.MinCreatedAt,
                Lte = query.MaxCreatedAt
            });
        }

        if (query.Tags != null && query.Tags.Any())
        {
            foreach (var tag in query.Tags)
            {
                filterQueries.Add(new TermQuery
                {
                    Field = "tags",
                    Value = tag
                });
            }
        }

        if (query.Topics != null && query.Topics.Any())
        {
            foreach (var topic in query.Topics)
            {
                filterQueries.Add(new TermQuery
                {
                    Field = "topics",
                    Value = topic
                });
            }
        }

        return await _elasticBaseService.SearchAsync(
            _indexName, 
            s => s
                .From((query.Page - 1) * query.PageSize)
                .Size(query.PageSize)
                .Query(q => q
                    .Bool(b => b
                        .Must(mustQueries)
                        .Filter(filterQueries)
                    )
                )
                .Sort(s => s.Score())
            , cancellationToken);
    }

    public async Task<AppResult> UpsertPostAsync(PostDocument post, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.UpsertAsync(_indexName, post.Id.ToString(), post, cancellationToken);
    }

    public async Task<AppResult> BulkPostsAsync(List<PostDocument> posts, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.BulkIndexDocumentAsync(_indexName, posts, cancellationToken);
    }

    public async Task<AppResult> DeletePostAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.DeleteAsync(_indexName, postId.ToString(), cancellationToken);
    }

    public async Task<AppResult> DeletePostsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var query = new TermQuery
        {
            Field = "authorId",
            Value = authorId.ToString()
        };
        return await _elasticBaseService.DeleteByQueryAsync(_indexName, 
            d => d
            .Query(q => q
                .Term(t => t
                    .Field("authorId")
                    .Value(authorId.ToString())
                )
            ), cancellationToken);
    }
}

using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Search.ContentManagement.Communitites.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Search.Documents;
using SNS.Infrastructure.Search.Abstractions;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Infrastructure.Search.Services;

public class CommunitySearchService : ICommunitySearchService
{
    private readonly IElasticDocumentService<CommunityDocument> _elasticBaseService;
    private readonly string _indexName = "sns_communities";

    public CommunitySearchService(IElasticDocumentService<CommunityDocument> elasticBaseService)
    {
        _elasticBaseService = elasticBaseService;
    }

    public async Task<SearchResult<CommunityDocument>> GetCommunitiesByIds(
        List<string> communityIds, 
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.SearchAsync(_indexName, s => s
        .Size(count)
        .Query(q => q
        .Ids(i => i.Values(communityIds.ToArray()))), 
        cancellationToken);
    }

    public async Task<SearchResult<CommunityDocument>> GetSuggestedCommunities(
        CancellationToken cancellationToken = default)
    {
        var filterQueries = new List<Query>
        {
            new TermQuery
            {
                Field = "type",
                Value = CommunityType.Public.ToString().ToLower()
            }
        };

        return await _elasticBaseService.SearchAsync(_indexName, s => s
            .Size(10)
            .Query(q => q
                .Bool(b => b
                    .Filter(filterQueries)
                )
            )
            .Sort(sort => sort
                .Field(f => f.MembersCount, fs => fs.Order(SortOrder.Desc))
            ),
            cancellationToken);
    }

    public async Task<SearchResult<CommunityDocument>> SearchCommunitiesAsync(
        CommunitySearchQuery query,
        CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Query = query.SearchTerm,
                Fields = new[] { "name^3.0", "description^2.0", "topics" },
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        if (query.Type.HasValue)
        {
            filterQueries.Add(new TermQuery
            {
                Field = "type",
                Value = query.Type.Value.ToString().ToLower()
            });
        }

        return await _elasticBaseService.SearchAsync(_indexName, s => s
            .From((query.Page - 1) * query.PageSize)
            .Size(query.PageSize)
            .Query(q => q
                .Bool(b => b
                    .Must(mustQueries)
                    .Filter(filterQueries)
                )
            )
            .Sort(sort => sort
                .Score()
                .Field(f => f.MembersCount, fs => fs.Order(SortOrder.Desc))
            ),
            cancellationToken);
    }


    public async Task<AppResult> UpsertCommunityAsync(CommunityDocument community, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.UpsertAsync(_indexName, community.Id.ToString(), community, cancellationToken);
    }

    public async Task<AppResult> DeleteCommunityAsync(string communityId, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.DeleteAsync(_indexName, communityId, cancellationToken);
    }
}

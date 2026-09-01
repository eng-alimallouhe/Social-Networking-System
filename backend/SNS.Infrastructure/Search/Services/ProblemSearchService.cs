using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Search.Discussions.Problems.Queries;
using SNS.Application.Search.Discussions.Problems.Queries.GetProblemsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using SNS.Infrastructure.Search.Abstractions;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Infrastructure.Search.Services;


public class ProblemSearchService : IProblemSearchService
{
    private readonly IElasticDocumentService<ProblemDocument> _elasticBaseService;
    private const string IndexName = "sns_problems";

    public ProblemSearchService(IElasticDocumentService<ProblemDocument> elasticBaseService)
    {
        _elasticBaseService = elasticBaseService;
    }

    // ==========================================
    // 1. البحث العادي (Search & Filter)
    // ==========================================
    public async Task<SearchResult<ProblemDocument>> SearchProblemsAsync(GetProblemsSearchQuery query, CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();


        // 2. البحث النصي (Title & ContentManagement)
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Query = query.SearchTerm,
                Fields = new[] { "title^3.0", "contentBlocks.content" },
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        if (query.Level.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "level", Value = (int)query.Level.Value });
        }

        if (query.Status.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "status", Value = (int)query.Status.Value });
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

        return await _elasticBaseService.SearchAsync(IndexName, s => s
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
                .Field(f => f.CreatedAt, fs => fs.Order(SortOrder.Desc))
            ),
            cancellationToken);
    }

    public async Task<SearchResult<ProblemDocument>> GetProblemFeedAsync(ProblemFeedParameter request, CancellationToken cancellationToken = default)
    {
        var shouldQueries = new List<Query>();
        var filterQueries = new List<Query>();
        var mustNotQueries = new List<Query>();

        filterQueries.Add(new DateRangeQuery { Field = "createdAt", Gte = request.StartDate });

        foreach (var problemId in request.ExcludedProblemsIds)
        {
            mustNotQueries.Add(new TermQuery { Field = "id", Value = problemId.ToString() });
        }

        var userInterests = request.Skills.Concat(request.Topics).Concat(request.Tags).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        if (userInterests.Any())
        {
            shouldQueries.Add(new MultiMatchQuery
            {
                Query = string.Join(" ", userInterests),
                Fields = new[] { "title^2.0", "contentBlocks.content" }
            });
        }

        return await _elasticBaseService.SearchAsync(IndexName, s => s
            .Size(request.FeedSize)
            .Query(q => q
                .Bool(b => b
                    .Filter(filterQueries)
                    .Should(shouldQueries)
                    .MustNot(mustNotQueries)
                )
            )
            .Sort(sort => sort.Score()),
            cancellationToken);
    }

    // ==========================================
    // 3. عمليات التزامن (Sync)
    // ==========================================
    public async Task<AppResult> UpsertProblemAsync(ProblemDocument problem, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.UpsertAsync(IndexName, problem.Id.ToString(), problem, cancellationToken);
    }

    public async Task<AppResult> DeleteProblemAsync(Guid problemId, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.DeleteAsync(IndexName, problemId.ToString(), cancellationToken);
    }

    public async Task<AppResult> BulkProblemsAsync(List<ProblemDocument> problems, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.BulkIndexDocumentAsync(IndexName, problems, cancellationToken);
    }

    public async Task<AppResult> DeleteProblemsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        var query = new Query
        {
            Term = new TermQuery
            {
                Field = "authorId",
                Value = authorId.ToString()
            }
        };

        return await _elasticBaseService.DeleteByQueryAsync(IndexName, 
            d => d
            .Query(q => q
                .Term(t => t
                    .Field("authorId")
                    .Value(authorId.ToString()))), cancellationToken);
    }
}

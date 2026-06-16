using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SNS.Application.Search.Jobs.Abstractions;
using SNS.Application.Search.Jobs.Queries;
using SNS.Application.Search.Jobs.Queries.GetJobsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using SNS.Infrastructure.Search.Abstractions;
using AppResult = SNS.Shared.Results.Result;


namespace SNS.Infrastructure.Search.Services;

public class JobSearchService : IJobSearchService
{
    private readonly IElasticDocumentService<JobsDocument> _elasticBaseService;
    private readonly string _indexName = "sns_jobs";

    public JobSearchService(IElasticDocumentService<JobsDocument> elasticBaseService)
    {
        _elasticBaseService = elasticBaseService;
    }

    public async Task<SearchResult<JobsDocument>> SearchJobsAsync(JobSearchQuery query, CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Query = query.SearchTerm,
                Fields = new[] { "title^3.0", "companyName^2.0", "location^1.5", "description" },
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        if (query.Type.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "type", Value = query.Type.Value.ToString().ToLower() });
        }

        if (query.SalaryType.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "salaryType", Value = query.SalaryType.Value.ToString().ToLower() });
        }

        if (query.MinSalary.HasValue)
        {
            filterQueries.Add(new NumberRangeQuery { Field = "maxSalary", Gte = (double)query.MinSalary.Value });
        }

        if (query.MaxSalary.HasValue)
        {
            filterQueries.Add(new NumberRangeQuery { Field = "minSalary", Lte = (double)query.MaxSalary.Value });
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
                .Field(f => f.CreatedAt, fs => fs.Order(SortOrder.Desc)) 
            ),
            cancellationToken);
    }

    public async Task<SearchResult<JobsDocument>> GetSuggestedJobsAsync(SuggestedJobsQuery query, CancellationToken cancellationToken = default)
    {
        var shouldQueries = new List<Query>();
        var filterQueries = new List<Query>();
        var mustNotQueries = new List<Query>();

        mustNotQueries.Add(new ExistsQuery { Field = "closedAt" });

        if (query.Skills.Any())
        {
            shouldQueries.Add(new MultiMatchQuery
            {
                Query = string.Join(" ", query.Skills),
                Fields = new[] { "title^2.0", "description" },
                Boost = 2 
            });
        }

        if (query.Topics.Any())
        {
            shouldQueries.Add(new MultiMatchQuery
            {
                Query = string.Join(" ", query.Topics),
                Fields = new[] { "title", "description" }
            });
        }

        return await _elasticBaseService.SearchAsync(_indexName, s => s
            .From((query.Page - 1) * query.PageSize)
            .Size(query.PageSize)
            .Query(q => q
                .FunctionScore(fs => fs
                    .Query(qb => qb
                        .Bool(b => b
                            .Filter(filterQueries)
                            .MustNot(mustNotQueries)
                            .Should(shouldQueries)
                            .MinimumShouldMatch(1)
                        )
                    )
                    .Functions(new List<FunctionScore>
                    {
                        new FunctionScore
                        {
                            Weight = 2,
                            Filter = new DateRangeQuery { Field = "createdAt", Gte = DateTime.UtcNow.AddDays(-7) } 
                        }
                    })
                    .BoostMode(FunctionBoostMode.Multiply)
                )
            )
            .Sort(sort => sort.Score()),
            cancellationToken);
    }

    public async Task<AppResult> UpsertJobAsync(JobsDocument job, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.UpsertAsync(_indexName, job.Id.ToString(), job, cancellationToken);
    }

    public async Task<AppResult> DeleteJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.DeleteAsync(_indexName, jobId.ToString(), cancellationToken);
    }
}

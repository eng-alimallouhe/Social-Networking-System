using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Search.Projects.Queries;
using SNS.Application.Search.Projects.Queries.GetProjectsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using SNS.Infrastructure.Search.Abstractions;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Infrastructure.Search.Services;

public class ProjectSearchService : IProjectSearchService
{
    private readonly IElasticDocumentService<ProjectDocument> _elasticBaseService;
    private readonly string _indexName = "sns_projects";

    public ProjectSearchService(IElasticDocumentService<ProjectDocument> elasticBaseService)
    {
        _elasticBaseService = elasticBaseService;
    }

    public async Task<SearchResult<ProjectDocument>> SearchProjectsAsync(ProjectSearchQuery query, CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Query = query.SearchTerm,
                Fields = new[] { "title^3.0", "shortDescription", "readmeContent^0.5" }, 
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        if (query.Status.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "status", Value = query.Status.Value.ToString().ToLower() });
        }

        if (query.RequiredSkills != null && query.RequiredSkills.Any())
        {
            foreach (var skill in query.RequiredSkills)
            {
                filterQueries.Add(new TermQuery { Field = "skills", Value = skill });
            }
        }

        if (query.MinContributors.HasValue || query.MaxContributors.HasValue)
        {
            filterQueries.Add(new NumberRangeQuery
            {
                Field = "contributorsCount",
                Gte = query.MinContributors.HasValue ? (double)query.MinContributors.Value : null,
                Lte = query.MaxContributors.HasValue ? (double)query.MaxContributors.Value : null
            });
        }

        if (query.MinRate.HasValue)
        {
            filterQueries.Add(new NumberRangeQuery { Field = "rate", Gte = (double)query.MinRate.Value });
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
                .Field(f => f.Rate, fs => fs.Order(SortOrder.Desc)) 
            ),
            cancellationToken);
    }

    public async Task<SearchResult<ProjectDocument>> GetSuggestedProjectsAsync(SuggestedProjectsQuery query, CancellationToken cancellationToken = default)
    {
        var shouldQueries = new List<Query>();
        var filterQueries = new List<Query>();

        filterQueries.Add(new TermQuery { Field = "isActive", Value = true });
        filterQueries.Add(new ExistsQuery { Field = "publishedAt" });

        if (query.UserSkills.Any())
        {
            shouldQueries.Add(new TermsQuery
            {
                Field = "skills",
                Terms = new TermsQueryField(query.UserSkills.Select(s => FieldValue.String(s)).ToArray())
            });

            shouldQueries.Add(new MultiMatchQuery
            {
                Query = string.Join(" ", query.UserSkills),
                Fields = new[] { "shortDescription", "readmeContent" },
                Boost = 1.0f 
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
                            .Should(shouldQueries)
                            .MinimumShouldMatch(1) 
                        )
                    )
                    .Functions(new List<FunctionScore>
                    {
                        new FunctionScore
                        {
                            FieldValueFactor = new FieldValueFactorScoreFunction
                            {
                                Field = "rate",
                                Factor = 1.5, 
                                Modifier = FieldValueFactorModifier.Log1p 
                            }
                        }
                    })
                    .ScoreMode(FunctionScoreMode.Multiply)
                )
            )
            .Sort(sort => sort.Score()), 
            cancellationToken);
    }

    public async Task<AppResult> UpsertProjectAsync(ProjectDocument project, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.UpsertAsync(_indexName, project.Id.ToString(), project, cancellationToken);
    }

    public async Task<AppResult> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.DeleteAsync(_indexName, projectId.ToString(), cancellationToken);
    }

    public async Task<AppResult> BulkProjectsAsync(IEnumerable<ProjectDocument> projects, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.BulkIndexDocumentAsync(_indexName, projects, cancellationToken);
    }
    public async Task<AppResult> DeleteProjectsByOnwerIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.DeleteByQueryAsync(
            _indexName,
            d => d
            .Query(q => q
                .Term(t => t
                    .Field(f => f.OwnerId)
                    .Value(authorId.ToString())
                )
            ), cancellationToken);
    }
}

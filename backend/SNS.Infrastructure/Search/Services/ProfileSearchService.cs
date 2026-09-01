using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SNS.Application.Search.Profiles.Profiles;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using SNS.Infrastructure.Search.Abstractions;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Infrastructure.Search.Services;

public class ProfileSearchService : IProfileSearchService
{
    private readonly IElasticDocumentService<ProfileDocument> _elasticBaseService;
    private const string IndexName = "sns_profiles";

    public ProfileSearchService(IElasticDocumentService<ProfileDocument> elasticBaseService)
    {
        _elasticBaseService = elasticBaseService;
    }

    public async Task<SearchResult<ProfileDocument>> SearchProfilesAsync(GetProfilesSearchQuery query, CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();
        var mustNotQueries = new List<Query>();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Query = query.SearchTerm,
                Fields = new[] { "fullName^2.0", "specialization", "bio" },
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        if (query.RequiredSkills != null && query.RequiredSkills.Any())
        {
            foreach (var skill in query.RequiredSkills)
            {
                // FIX: Object Initializer & camelCase
                filterQueries.Add(new TermQuery { Field = "skills", Value = skill });
            }
        }

        if (query.CurrentProfileId != null)
        {
            mustNotQueries.Add(new TermQuery { Field = "blackList", Value = query.CurrentProfileId.Value.ToString() });
        }

        return await _elasticBaseService.SearchAsync(IndexName, s => s
            .From((query.Page - 1) * query.PageSize)
            .Size(query.PageSize)
            .Query(q => q
                .Bool(b => b
                    .Must(mustQueries)
                    .Filter(filterQueries)
                    .MustNot(mustNotQueries)
                )
            )
            .Sort(sort => sort
                .Score()
                .Field(f => f.CreatedAt, fs => fs.Order(SortOrder.Desc))
            ),
            cancellationToken);
    }

    public async Task<SearchResult<ProfileDocument>> GetSuggestedProfilesAsync(
        SuggestedProfilesRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var shouldQueries = new List<Query>();
        var mustNotQueries = new List<Query>();

        foreach (var skill in request.Skills)
        {
            shouldQueries.Add(new TermQuery { Field = "skills", Value = skill });
        }

        foreach (var university in request.Universities)
        {
            shouldQueries.Add(new TermQuery { Field = "universities", Value = university });
        }

        foreach (var exclude in request.ExcludedIds)
        {
            mustNotQueries.Add(new TermQuery { Field = "id", Value = exclude.ToString() });
        }

        return await _elasticBaseService.SearchAsync(IndexName, s => s
            .From((request.Page - 1) * request.PageSize)
            .Size(request.PageSize)
            .Query(q => q
                .Bool(b => b
                    .Should(shouldQueries)
                    .MustNot(mustNotQueries)
                )
            )
            .Sort(sort => sort.Score()),
            cancellationToken);
    }

    public async Task<AppResult> UpsertProfileAsync(ProfileDocument profile, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.UpsertAsync(IndexName, profile.Id.ToString(), profile, cancellationToken);
    }

    public async Task<AppResult> DeleteProfile(Guid profileId, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.DeleteAsync(IndexName, profileId.ToString(), cancellationToken);
    }

    public async Task<AppResult> BulkProfilesAsync(List<ProfileDocument> profiles, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.BulkIndexDocumentAsync(IndexName, profiles, cancellationToken);
    } 
}

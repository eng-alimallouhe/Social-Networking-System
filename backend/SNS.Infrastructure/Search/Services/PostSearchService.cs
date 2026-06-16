using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.ContentManagement.Communities.Enums;
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

    public async Task<SearchResult<PostDocument>> SearchAsync(PostSearchQuery query, CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Fields = new[] { "userName", "fullName" },
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


    public async Task<SearchResult<PostDocument>> GetFeedPostsAsync(
        FeedRequestParameter parameter,
        CancellationToken cancellationToken = default)
    {
        var filterQueries = new List<Query>();
        var shouldQueries = new List<Query>();
        var mustNotQueries = new List<Query>();
        var functions = new List<FunctionScore>();

        foreach (var profileId in parameter.ExcludedProfilesIds)
        {
            mustNotQueries.Add(new TermQuery { Field = "authorId", Value = profileId.ToString() });
        }

        foreach (var postId in parameter.ExcludedPostsIds)
        {
            mustNotQueries.Add(new TermQuery { Field = "id", Value = postId.ToString() });
        }

        filterQueries.Add(new TermQuery
        {
            Field = "communityType",
            Value = CommunityType.Public.ToString().ToLower()
        });

        filterQueries.Add(new BoolQuery
        {
            Should = new List<Query>
        {
            new DateRangeQuery { Field = "createdAt", Gte = parameter.StartDate },
            new DateRangeQuery { Field = "lastInteractedAt", Gte = parameter.StartDate }
        },
            MinimumShouldMatch = 1
        });

        if (parameter.CommunitiesIds.Any())
        {
            shouldQueries.Add(new TermsQuery
            {
                Field = "communityId",
                Terms = new TermsQueryField(parameter.CommunitiesIds.Select(id => FieldValue.String(id.ToString())).ToArray())
            });
        }

        if (parameter.Topics.Any())
        {
            shouldQueries.Add(new TermsQuery
            {
                Field = "topics",
                Terms = new TermsQueryField(parameter.Topics.Select(t => FieldValue.String(t)).ToArray())
            });
        }

        if (parameter.FollowedProfilesIds.Any())
        {
            shouldQueries.Add(new TermsQuery
            {
                Field = "authorId",
                Terms = new TermsQueryField(parameter.FollowedProfilesIds.Select(id => FieldValue.String(id.ToString())).ToArray()),
                Boost = 4
            });
        }

        
        if (parameter.Skills.Any())
        {
            shouldQueries.Add(new MultiMatchQuery
            {
                Query = string.Join(" ", parameter.Skills),
                Fields = new[] { "content", "title" },
                Boost = 3,
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        functions.Add(new FunctionScore
        {
            FieldValueFactor = new FieldValueFactorScoreFunction
            {
                Field = "viewsCount",
                Factor = 0.02,
                Modifier = FieldValueFactorModifier.Log1p
            }
        });

        functions.Add(new FunctionScore
        {
            FieldValueFactor = new FieldValueFactorScoreFunction
            {
                Field = "reactionsCount",
                Factor = 0.1,
                Modifier = FieldValueFactorModifier.Log1p
            }
        });

        functions.Add(new FunctionScore
        {
            FieldValueFactor = new FieldValueFactorScoreFunction
            {
                Field = "commentsCount",
                Factor = 0.1,
                Modifier = FieldValueFactorModifier.Log1p
            }
        });

        return await _elasticBaseService.SearchAsync(
            _indexName,
            s => s
                .Size(parameter.FeedSize)
                .Query(q => q
                    .FunctionScore(fs => fs
                        .Query(qb => qb
                            .Bool(b => b
                                .Filter(filterQueries)
                                .MustNot(mustNotQueries)
                                .Should(shouldQueries)
                                .MinimumShouldMatch(0)
                            )
                        )
                        .Functions(functions)
                        .ScoreMode(FunctionScoreMode.Sum)
                        .BoostMode(FunctionBoostMode.Sum)
                    )
                )
                .Sort(s => s.Score())
            ,
            cancellationToken);
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

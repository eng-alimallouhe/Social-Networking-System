using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Identity.Users.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Search.Documents;
using SNS.Infrastructure.Search.Abstractions;
using System.Linq.Expressions;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Infrastructure.Search.Services;

public class UserSearchService : IUserSearchService
{
    private readonly IElasticDocumentService<UserDocument> _elasticBaseService;
    private readonly string _indexName = "sns_users";

    public UserSearchService(IElasticDocumentService<UserDocument> elasticBaseService)
    {
        _elasticBaseService = elasticBaseService;
    }

    public async Task<SearchResult<UserDocument>> SearchUsersAsync(UserSearchQuery query, CancellationToken cancellationToken = default)
    {
        var mustQueries = new List<Query>();
        var filterQueries = new List<Query>();
        var mustNotQueries = new List<Query>();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            mustQueries.Add(new MultiMatchQuery
            {
                Fields = new[] {"userName", "fullName"},
                Query = query.SearchTerm,
                Fuzziness = new Fuzziness("AUTO")
            });
        }

        if (!string.IsNullOrEmpty(query.Role))
        {
            filterQueries.Add(new TermQuery { Field = "role", Value = query.Role });
        }

        if (query.IsBanned.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "Status", Value = UserStatus.PermanentlyBanned.ToString() });
        }

        if (query.IsActive.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "Status", Value = UserStatus.Active.ToString() });
        }

        if (query.IsSuspended.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "Status", Value = UserStatus.Suspended.ToString() });
        }

        if (query.IsVerified.HasValue)
        {
            filterQueries.Add(new TermQuery { Field = "isVerified", Value = query.IsVerified.Value });
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

        var sortConfig = DetectSort(query.SortBy, query.SortDirection);

        return await _elasticBaseService.SearchAsync(
            _indexName, 
            a => a
            .From((query.Page - 1) * query.PageSize)
            .Size(query.PageSize)
            .Query(q => q
                .Bool(b => b
                    .Must(mustQueries.ToArray())
                    .Filter(filterQueries.ToArray())
                    .MustNot(mustNotQueries.ToArray())
                )
            )
            .Sort(sort => sort
                .Score()
                .Field(sortConfig.Field, fs => fs.Order(sortConfig.Order))
            ),
            cancellationToken);
    }

    public async Task<AppResult> UpsertUserAsync(UserDocument user, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.UpsertAsync(_indexName, user.Id.ToString(), user, cancellationToken);
    }

    public async Task<AppResult> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.DeleteAsync(_indexName, userId.ToString(), cancellationToken);
    }


    private (Expression<Func<UserDocument, object?>> Field, SortOrder Order)
    DetectSort(UserSearchSortBy sortBy, SortDirection direction)
    {
        var sortOrder = direction == SortDirection.Ascending
            ? SortOrder.Asc
            : SortOrder.Desc;

        Expression<Func<UserDocument, object?>> field = sortBy switch
        {
            UserSearchSortBy.Role => x => x.Role,
            UserSearchSortBy.CreatedAt => x => x.CreatedAt,
            UserSearchSortBy.LastLogin => x => x.LastLogin,
            UserSearchSortBy.IsBanned => x => x.Status,

            _ => x => x.CreatedAt
        };

        return (field, sortOrder);
    }

    public async Task<AppResult> BulkUsersAsync(List<UserDocument> documents, CancellationToken cancellationToken = default)
    {
        return await _elasticBaseService.BulkIndexDocumentAsync(_indexName, documents, cancellationToken);
    }
}

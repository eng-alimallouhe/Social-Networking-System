using SNS.Application.Search.Identity.Users.Queries.GetUsersSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Identity.Users.Abstractions;

public interface IUserSearchService
{
    Task<Result> UpsertUserAsync(UserDocument userDocument, CancellationToken cancellationToken = default);

    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SearchResult<UserDocument>> SearchUsersAsync(GetUsersSearchQuery query, CancellationToken cancellationToken = default);

    Task<Result> BulkUsersAsync(List<UserDocument> documents, CancellationToken cancellationToken = default);
}

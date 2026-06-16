using SNS.Application.Search.Identity.Users.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Identity.Users.Abstractions;

public interface IUserSearchService
{
    Task<Result> UpsertUserAsync(UserDocument userDocument, CancellationToken cancellationToken = default);

    Task<Result> DeleteUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<SearchResult<UserDocument>> SearchUsersAsync(UserSearchQuery query, CancellationToken cancellationToken = default);
}

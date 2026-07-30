using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Identity.Users.Queries.GetUsersSearch;

/// <summary>
/// Represents a search query to search user documents in the search index using specified filter criteria.
/// </summary>
/// <param name="Parameters">The search filter, sorting, and pagination parameters for users.</param>
public sealed record GetUsersSearchQuery(UserSearchQuery Parameters)
: IQuery<SearchResult<UserDocument>>;


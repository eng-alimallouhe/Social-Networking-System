using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Identity.Users.Queries.GetUsersSearch;

public sealed record GetUsersSearchQuery(UserSearchQuery Parameters)
: IQuery<SearchResult<UserDocument>>;

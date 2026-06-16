using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;

public sealed record GetProfilesSearchQuery(ProfileSearchQuery Parameters)
: IQuery<SearchResult<ProfileDocument>>;

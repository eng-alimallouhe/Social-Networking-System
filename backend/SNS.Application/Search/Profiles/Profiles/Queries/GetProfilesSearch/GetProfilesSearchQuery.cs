using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;

/// <summary>
/// Represents a search query to search profile documents in the search index using specified filter criteria.
/// </summary>
/// <param name="Parameters">The search filter, sorting, and pagination parameters for user profiles.</param>
public sealed record GetProfilesSearchQuery(ProfileSearchQuery Parameters)
: IQuery<SearchResult<ProfileDocument>>;


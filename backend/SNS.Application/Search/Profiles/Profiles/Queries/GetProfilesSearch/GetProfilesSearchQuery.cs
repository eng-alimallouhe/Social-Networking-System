using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;

/// <summary>
/// Represents a search query to search profile documents in the search index using specified filter criteria.
/// </summary>
/// <param name="SearchTerm">Optional keyword matching full name, bio, or specialization.</param>
/// <param name="RequiredSkills">Optional list of required skill names to filter profiles.</param>
/// <param name="CurrentProfileId">Optional current user profile ID to exclude blocked profiles.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of profiles returned per page.</param>
public sealed record GetProfilesSearchQuery(
    string? SearchTerm = null,
    List<string>? RequiredSkills = null,
    Guid? CurrentProfileId = null,
    int Page = 1,
    int PageSize = 10
) : IQuery<SearchResult<ProfileSummaryDto>>;

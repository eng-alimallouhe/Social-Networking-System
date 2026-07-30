namespace SNS.Application.Search.Profiles.Profiles;

/// <summary>
/// Represents request parameters to retrieve recommended profile suggestions based on user profile details.
/// </summary>
/// <param name="profileId">The unique identifier of the profile requesting suggestions.</param>
/// <param name="ExcludedIds">List of profile identifiers to exclude from suggestions.</param>
/// <param name="Skills">List of skill names to match against candidate profiles.</param>
/// <param name="Universities">List of university names to match for academic affinity.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of profile suggestions to return per page.</param>
public sealed record SuggestedProfilesRequestDto(
    Guid profileId,
    List<Guid> ExcludedIds,
    List<string> Skills,
    List<string> Universities,
    int Page = 1,
    int PageSize = 10
);


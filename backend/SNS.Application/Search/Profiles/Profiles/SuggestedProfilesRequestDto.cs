namespace SNS.Application.Search.Profiles.Profiles;

public sealed record SuggestedProfilesRequestDto(
    Guid profileId,
    List<Guid> ExcludedIds,
    List<string> Skills,
    List<string> Universities,
    int Page = 1,
    int PageSize = 10
);

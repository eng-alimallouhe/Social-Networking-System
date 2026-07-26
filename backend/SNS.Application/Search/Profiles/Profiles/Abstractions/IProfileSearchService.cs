using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;
using SNS.Domain.Search.Documents;
using SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;

namespace SNS.Application.Search.Profiles.Profiles.Abstractions;

public interface IProfileSearchService
{
    Task<Result> UpsertProfileAsync(ProfileDocument profile, CancellationToken cancellationToken = default);

    Task<Result> DeleteProfile(Guid profileId, CancellationToken cancellationToken = default);

    Task<SearchResult<ProfileDocument>> SearchProfilesAsync(ProfileSearchQuery query, CancellationToken cancellationToken = default);

    Task<SearchResult<ProfileDocument>> GetSuggestedProfilesAsync(SuggestedProfilesRequestDto request, CancellationToken cancellationToken = default);

    Task<Result> BulkProfilesAsync(List<ProfileDocument> profiles, CancellationToken cancellationToken = default);
}

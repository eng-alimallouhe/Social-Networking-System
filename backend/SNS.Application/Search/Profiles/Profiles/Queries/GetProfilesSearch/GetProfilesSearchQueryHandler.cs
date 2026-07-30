using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;

/// <summary>
/// Handles the execution of <see cref="GetProfilesSearchQuery"/> to search profile documents.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Invokes <see cref="IProfileSearchService"/> passing request search parameters.
/// 2. Returns <see cref="SearchResult{ProfileDocument}"/> matching user profiles.
/// </remarks>
public class GetProfilesSearchQueryHandler
: IQueryHandler<GetProfilesSearchQuery, SearchResult<ProfileDocument>>
{
    private readonly IProfileSearchService _profileSearchService;

    public GetProfilesSearchQueryHandler(IProfileSearchService profileSearchService)
    {
        _profileSearchService = profileSearchService;
    }

    public async Task<Result<SearchResult<ProfileDocument>>> Handle(
        GetProfilesSearchQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _profileSearchService.SearchProfilesAsync(request.Parameters, cancellationToken);

        return Result<SearchResult<ProfileDocument>>.Success(result, OperationStatusCode.Success);
    }
}

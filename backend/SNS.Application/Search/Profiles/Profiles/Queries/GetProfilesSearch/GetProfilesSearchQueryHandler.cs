using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;

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

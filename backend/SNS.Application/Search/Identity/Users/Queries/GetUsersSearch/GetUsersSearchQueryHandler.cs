using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;
using SNS.Domain.Search.Documents;
using SNS.Shared.StatusCodes;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.Identity.Users.Queries.GetUsersSearch;

public class GetUsersSearchQueryHandler
: IQueryHandler<GetUsersSearchQuery, SearchResult<UserDocument>>
{
    private readonly IUserSearchService _userSearchService;

    public GetUsersSearchQueryHandler(IUserSearchService userSearchService)
    {
        _userSearchService = userSearchService;
    }

    public async Task<Result<SearchResult<UserDocument>>> Handle(
        GetUsersSearchQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _userSearchService.SearchUsersAsync(request.Parameters, cancellationToken);

        return Result<SearchResult<UserDocument>>.Success(result, OperationStatusCode.Success);
    }
}

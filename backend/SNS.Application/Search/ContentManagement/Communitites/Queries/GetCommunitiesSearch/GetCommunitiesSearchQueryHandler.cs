using MediatR;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;
using SNS.Domain.Search.Documents;
using SNS.Shared.StatusCodes;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.ContentManagement.Communitites.Queries.GetCommunitiesSearch;

public class GetCommunitiesSearchQueryHandler
: IQueryHandler<GetCommunitiesSearchQuery, SearchResult<CommunityDocument>>
{
    private readonly ICommunitySearchService _communitySearchService;

    public GetCommunitiesSearchQueryHandler(ICommunitySearchService communitySearchService)
    {
        _communitySearchService = communitySearchService;
    }

    public async Task<Result<SearchResult<CommunityDocument>>> Handle(
        GetCommunitiesSearchQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _communitySearchService.SearchCommunitiesAsync(request.Parameters, cancellationToken);

        return Result<SearchResult<CommunityDocument>>.Success(result, OperationStatusCode.Success);
    }
}

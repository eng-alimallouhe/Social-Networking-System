using MediatR;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;
using SNS.Domain.Search.Documents;
using SNS.Shared.StatusCodes;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.ContentManagement.Communitites.Queries.GetCommunitiesSearch;

/// <summary>
/// Handles the execution of <see cref="GetCommunitiesSearchQuery"/> to search community documents.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Invokes <see cref="ICommunitySearchService"/> passing request search parameters.
/// 2. Returns <see cref="SearchResult{CommunityDocument}"/> search results.
/// </remarks>
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

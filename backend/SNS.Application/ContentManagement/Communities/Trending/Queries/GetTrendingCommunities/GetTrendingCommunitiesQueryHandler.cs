using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Communities.Trending.Abstractions;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Communities.Trending.Queries.GetTrendingCommunities;

/// <summary>
/// Handles the execution of <see cref="GetTrendingCommunitiesQuery"/> to retrieve trending communities.
/// </summary>
internal sealed class GetTrendingCommunitiesQueryHandler : IQueryHandler<GetTrendingCommunitiesQuery, List<CommunitySummaryDto>>
{
    private readonly ITrendingCommunityService _trendingService;

    public GetTrendingCommunitiesQueryHandler(ITrendingCommunityService trendingService)
    {
        _trendingService = trendingService;
    }

    public async Task<Result<List<CommunitySummaryDto>>> Handle(GetTrendingCommunitiesQuery request, CancellationToken cancellationToken)
    {
        var count = request.Count > 0 ? request.Count : 10;
        if (count > 50) count = 50;

        var trending = await _trendingService.GetTrendingCommunitiesAsync(count, cancellationToken);
        return Result<List<CommunitySummaryDto>>.Success(trending, OperationStatusCode.Success);
    }
}

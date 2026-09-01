using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.ContentManagement.Communitites.Queries.GetCommunitiesSearch;

/// <summary>
/// Handles the execution of <see cref="GetCommunitiesSearchQuery"/> to search communities and return authoritative community summaries.
/// </summary>
public class GetCommunitiesSearchQueryHandler
: IQueryHandler<GetCommunitiesSearchQuery, SearchResult<CommunitySummaryDto>>
{
    private readonly ICommunitySearchService _communitySearchService;
    private readonly IApplicationDbContext _dbContext;

    public GetCommunitiesSearchQueryHandler(
        ICommunitySearchService communitySearchService,
        IApplicationDbContext dbContext)
    {
        _communitySearchService = communitySearchService;
        _dbContext = dbContext;
    }

    public async Task<Result<SearchResult<CommunitySummaryDto>>> Handle(
        GetCommunitiesSearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchResult = await _communitySearchService.SearchCommunitiesAsync(request, cancellationToken);
        if (!searchResult.Hits.Any())
        {
            return Result<SearchResult<CommunitySummaryDto>>.Success(new SearchResult<CommunitySummaryDto>
            {
                Hits = new List<SearchHit<CommunitySummaryDto>>(),
                Total = searchResult.Total
            }, OperationStatusCode.Success);
        }

        var communityIds = searchResult.Hits.Select(h => h.Document.Id).ToList();

        var communities = await _dbContext.Communities
            .Where(c => communityIds.Contains(c.Id))
            .Select(c => new CommunitySummaryDto(
                c.Id,
                c.Name,
                c.Description,
                c.Type,
                c.LogoObjectKey,
                c.Memberships.Count(),
                c.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var orderedHits = searchResult.Hits
            .Select(hit =>
            {
                var communityDto = communities.FirstOrDefault(c => c.Id == hit.Document.Id);
                return communityDto != null ? new SearchHit<CommunitySummaryDto>(communityDto, hit.Score) : null;
            })
            .Where(h => h != null)
            .Select(h => h!)
            .ToList();

        return Result<SearchResult<CommunitySummaryDto>>.Success(new SearchResult<CommunitySummaryDto>
        {
            Hits = orderedHits,
            Total = searchResult.Total
        }, OperationStatusCode.Success);
    }
}

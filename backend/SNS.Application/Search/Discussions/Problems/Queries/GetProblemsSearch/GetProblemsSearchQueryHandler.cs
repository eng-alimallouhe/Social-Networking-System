using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Discussions.Problems.Queries.GetProblemsSearch;

/// <summary>
/// Handles the execution of <see cref="GetProblemsSearchQuery"/> to search discussion problems and return authoritative problem summaries.
/// </summary>
public class GetProblemsSearchQueryHandler
: IQueryHandler<GetProblemsSearchQuery, SearchResult<ProblemSummaryDto>>
{
    private readonly IProblemSearchService _problemSearchService;
    private readonly IApplicationDbContext _dbContext;

    public GetProblemsSearchQueryHandler(
        IProblemSearchService problemSearchService,
        IApplicationDbContext dbContext)
    {
        _problemSearchService = problemSearchService;
        _dbContext = dbContext;
    }

    public async Task<Result<SearchResult<ProblemSummaryDto>>> Handle(
        GetProblemsSearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchResult = await _problemSearchService.SearchProblemsAsync(request, cancellationToken);
        if (!searchResult.Hits.Any())
        {
            return Result<SearchResult<ProblemSummaryDto>>.Success(new SearchResult<ProblemSummaryDto>
            {
                Hits = new List<SearchHit<ProblemSummaryDto>>(),
                Total = searchResult.Total
            }, OperationStatusCode.Success);
        }

        var problemIds = searchResult.Hits.Select(h => h.Document.Id).ToList();

        var problems = await _dbContext.Problems
            .Where(p => problemIds.Contains(p.Id))
            .Select(p => new ProblemSummaryDto(
                p.Id,
                p.Title,
                p.Status,
                p.Level,
                p.AuthorId,
                p.Author.FullName,
                p.Author.ProfilePictureObjectKey,
                p.Votes.Count(v => v.Type == VoteType.Upvote),
                p.Solutions.Count(),
                p.ProblemTags.Select(pt => pt.Tag.Name).ToList(),
                p.ProblemTopics.Select(pt => pt.Topic.Name).ToList(),
                p.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        var orderedHits = searchResult.Hits
            .Select(hit =>
            {
                var problemDto = problems.FirstOrDefault(p => p.Id == hit.Document.Id);
                return problemDto != null ? new SearchHit<ProblemSummaryDto>(problemDto, hit.Score) : null;
            })
            .Where(h => h != null)
            .Select(h => h!)
            .ToList();

        return Result<SearchResult<ProblemSummaryDto>>.Success(new SearchResult<ProblemSummaryDto>
        {
            Hits = orderedHits,
            Total = searchResult.Total
        }, OperationStatusCode.Success);
    }
}

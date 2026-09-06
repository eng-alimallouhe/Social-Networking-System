using Microsoft.EntityFrameworkCore;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Discussions.Problems.Enums;
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
    private readonly IFileStorageService _fileStorageService;

    public GetProblemsSearchQueryHandler(
        IProblemSearchService problemSearchService,
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _problemSearchService = problemSearchService;
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
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

        var rawProblems = await _dbContext.Problems
            .AsNoTracking()
            .Where(p => problemIds.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.Status,
                p.Level,
                p.AuthorId,
                AuthorFullName = p.Author.FullName,
                AuthorProfilePictureObjectKey = p.Author.ProfilePictureObjectKey,
                UpvotesCount = p.Votes.Count(v => v.Type == VoteType.Upvote),
                SolutionsCount = p.Solutions.Count(s => s.IsActive),
                Tags = p.ProblemTags.Select(pt => pt.Tag.Name).ToList(),
                Topics = p.ProblemTopics.Select(pt => pt.Topic.Name).ToList(),
                p.CreatedAt,
                ContentBlocks = p.ContentBlocks
                    .OrderBy(cb => cb.Order)
                    .Select(cb => new
                    {
                        cb.Id,
                        cb.Type,
                        cb.Content,
                        cb.ExtraInfo,
                        cb.Order
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var problems = rawProblems.Select(p => new ProblemSummaryDto(
            p.Id,
            p.Title,
            p.Status,
            p.Level,
            p.AuthorId,
            p.AuthorFullName,
            p.AuthorProfilePictureObjectKey != null ? _fileStorageService.GetFilePublicUrl(p.AuthorProfilePictureObjectKey) : null,
            p.UpvotesCount,
            p.SolutionsCount,
            p.Tags,
            p.Topics,
            p.CreatedAt,
            p.ContentBlocks.Select(cb => new ProblemContentBlockDto(
                cb.Id,
                cb.Type,
                (cb.Type == ProblemBlockType.Image || cb.Type == ProblemBlockType.Video) && !string.IsNullOrWhiteSpace(cb.Content)
                    ? _fileStorageService.GetFilePublicUrl(cb.Content)
                    : cb.Content,
                cb.ExtraInfo,
                cb.Order
            )).ToList()
        )).ToList();

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

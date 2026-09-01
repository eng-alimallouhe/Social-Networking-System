
using Microsoft.EntityFrameworkCore;
using SNS.Application.Search.Jobs.Abstractions;
using SNS.Application.Search.Jobs.Contracts;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Jobs.Queries.GetJobsSearch;

/// <summary>
/// Handles the execution of <see cref="GetJobsSearchQuery"/> to search jobs and return authoritative job summaries.
/// </summary>
public class GetJobsSearchQueryHandler
: IQueryHandler<GetJobsSearchQuery, SearchResult<JobSummaryDto>>
{
    private readonly IJobSearchService _jobSearchService;
    private readonly IApplicationDbContext _dbContext;

    public GetJobsSearchQueryHandler(
        IJobSearchService jobSearchService,
        IApplicationDbContext dbContext)
    {
        _jobSearchService = jobSearchService;
        _dbContext = dbContext;
    }

    public async Task<Result<SearchResult<JobSummaryDto>>> Handle(
        GetJobsSearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchResult = await _jobSearchService.SearchJobsAsync(request, cancellationToken);
        if (!searchResult.Hits.Any())
        {
            return Result<SearchResult<JobSummaryDto>>.Success(new SearchResult<JobSummaryDto>
            {
                Hits = new List<SearchHit<JobSummaryDto>>(),
                Total = searchResult.Total
            }, OperationStatusCode.Success);
        }

        var jobIds = searchResult.Hits.Select(h => h.Document.Id).ToList();

        var jobs = await _dbContext.Jobs
            .Where(j => jobIds.Contains(j.Id))
            .Select(j => new JobSummaryDto(
                j.Id,
                j.Title,
                j.Description,
                j.Location,
                j.Type,
                j.MinSalary,
                j.MaxSalary,
                j.CurrencyCode,
                j.SalaryType,
                j.Company.Name,
                j.CreatedAt,
                j.ClosedAt
            ))
            .ToListAsync(cancellationToken);

        var orderedHits = searchResult.Hits
            .Select(hit =>
            {
                var jobDto = jobs.FirstOrDefault(j => j.Id == hit.Document.Id);
                return jobDto != null ? new SearchHit<JobSummaryDto>(jobDto, hit.Score) : null;
            })
            .Where(h => h != null)
            .Select(h => h!)
            .ToList();

        return Result<SearchResult<JobSummaryDto>>.Success(new SearchResult<JobSummaryDto>
        {
            Hits = orderedHits,
            Total = searchResult.Total
        }, OperationStatusCode.Success);
    }
}

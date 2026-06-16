using SNS.Application.Search.Jobs.Queries;
using SNS.Application.Search.Jobs.Queries.GetJobsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Application.Search.Jobs.Abstractions;

public interface IJobSearchService
{
    Task<SearchResult<JobsDocument>> SearchJobsAsync(JobSearchQuery query, CancellationToken cancellationToken = default);
    Task<SearchResult<JobsDocument>> GetSuggestedJobsAsync(SuggestedJobsQuery query, CancellationToken cancellationToken = default);
    Task<AppResult> UpsertJobAsync(JobsDocument job, CancellationToken cancellationToken = default);
    Task<AppResult> DeleteJobAsync(Guid jobId, CancellationToken cancellationToken = default);
}

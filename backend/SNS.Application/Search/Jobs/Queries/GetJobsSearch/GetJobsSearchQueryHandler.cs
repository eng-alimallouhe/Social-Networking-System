using SNS.Application.Search.Jobs.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Jobs.Queries.GetJobsSearch;

public class GetJobsSearchQueryHandler
    : IQueryHandler<GetJobsSearchQuery, SearchResult<JobsDocument>>
{
    private readonly IJobSearchService _jobSearchService;

    public GetJobsSearchQueryHandler(IJobSearchService jobSearchService)
    {
        _jobSearchService = jobSearchService;
    }

    public async Task<Result<SearchResult<JobsDocument>>> Handle(
        GetJobsSearchQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _jobSearchService.SearchJobsAsync(request.Parameters, cancellationToken);

        return Result<SearchResult<JobsDocument>>.Success(result, OperationStatusCode.Success);
    }
}

using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Jobs.Queries.GetJobsSearch;

public sealed record GetJobsSearchQuery(JobSearchQuery Parameters)
: IQuery<SearchResult<JobsDocument>>;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Jobs.Queries.GetJobsSearch;

/// <summary>
/// Represents a search query to search job documents in the search index using specified filter parameters.
/// </summary>
/// <param name="Parameters">The search filter, sorting, and pagination parameters for jobs.</param>
public sealed record GetJobsSearchQuery(JobSearchQuery Parameters)
: IQuery<SearchResult<JobsDocument>>;
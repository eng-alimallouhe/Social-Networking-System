using SNS.Application.Search.Discussions.Problems.Queries;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Application.Search.Discussions.Problems.Abstractions;

public interface IProblemSearchService
{
    Task<AppResult> DeleteProblemAsync(Guid problemId, CancellationToken cancellationToken = default);
    Task<SearchResult<ProblemDocument>> GetProblemFeedAsync(ProblemFeedParameter request, CancellationToken cancellationToken = default);
    Task<SearchResult<ProblemDocument>> SearchProblemsAsync(ProblemSearchQuery query, CancellationToken cancellationToken = default);
    Task<AppResult> UpsertProblemAsync(ProblemDocument problem, CancellationToken cancellationToken = default);
    Task<AppResult> BulkProblemsAsync(List<ProblemDocument> problems, CancellationToken cancellationToken = default);

    Task<AppResult> DeleteProblemsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
}

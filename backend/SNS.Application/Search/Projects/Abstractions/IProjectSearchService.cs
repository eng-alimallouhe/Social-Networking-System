using SNS.Application.Search.Projects.Queries;
using SNS.Application.Search.Projects.Queries.GetProjectsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Domain.Search.Documents;
using AppResult = SNS.Shared.Results.Result;

namespace SNS.Application.Search.Projects.Abstractions;

public interface IProjectSearchService
{
    Task<SearchResult<ProjectDocument>> SearchProjectsAsync(GetProjectsSearchQuery query, CancellationToken cancellationToken = default);
    Task<SearchResult<ProjectDocument>> GetSuggestedProjectsAsync(SuggestedProjectsQuery query, CancellationToken cancellationToken = default);
    Task<AppResult> UpsertProjectAsync(ProjectDocument project, CancellationToken cancellationToken = default);
    Task<AppResult> DeleteProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<AppResult> BulkProjectsAsync(IEnumerable<ProjectDocument> projects, CancellationToken cancellationToken = default);
    Task<AppResult> DeleteProjectsByOnwerIdAsync(Guid authorId, CancellationToken cancellationToken = default);
}

using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Domain.Search.Documents;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Projects.Queries.GetProjectsSearch;

/// <summary>
/// Handles the execution of <see cref="GetProjectsSearchQuery"/> to search project documents.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Invokes <see cref="IProjectSearchService"/> passing request search parameters.
/// 2. Returns <see cref="SearchResult{ProjectDocument}"/> matching projects.
/// </remarks>
public class GetProjectsSearchQueryHandler
: IQueryHandler<GetProjectsSearchQuery, SearchResult<ProjectDocument>>
{
    private readonly IProjectSearchService _projectSearchService;

    public GetProjectsSearchQueryHandler(IProjectSearchService projectSearchService)
    {
        _projectSearchService = projectSearchService;
    }

    public async Task<Result<SearchResult<ProjectDocument>>> Handle(
        GetProjectsSearchQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _projectSearchService.SearchProjectsAsync(request.Parameters, cancellationToken);

        return Result<SearchResult<ProjectDocument>>.Success(result, OperationStatusCode.Success);
    }
}

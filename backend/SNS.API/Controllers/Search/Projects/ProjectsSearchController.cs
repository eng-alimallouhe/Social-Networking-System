using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Projects.Contracts;
using SNS.Application.Search.Projects.Queries.GetProjectsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Search.Projects;

/// <summary>
/// Handles search operations for projects.
/// </summary>
[Route("api/v{version:apiVersion}/search/projects")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProjectsSearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsSearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Searches projects based on keywords, status, date range, required skills, contributor counts, rate, and pagination.
    /// </summary>
    /// <param name="query">The project search query parameters.</param>
    /// <response code="200">Returns paginated project overview search results <see cref="SearchResult{ProjectOverviewDto}"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<SearchResult<ProjectOverviewDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<SearchResult<ProjectOverviewDto>>>> SearchProjectsAsync([FromQuery] GetProjectsSearchQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}

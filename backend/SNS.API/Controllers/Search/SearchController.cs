using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Search.Queries.GetGlobalSearch;
using SNS.Application.Search.Queries.GlobalSearch;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Search;

/// <summary>
/// Handles unified global search across all searchable resources in the platform.
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Executes a global search query across profiles, projects, jobs, communities, problems, and posts.
    /// </summary>
    /// <param name="query">The global search query parameters.</param>
    /// <response code="200">Returns aggregated search results grouped by category <see cref="GlobalSearchResultDto"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<GlobalSearchResultDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<GlobalSearchResultDto>>> GetGlobalSearchAsync([FromQuery] GetGlobalSearchQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}

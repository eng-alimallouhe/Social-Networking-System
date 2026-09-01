using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Search.Discussions.Problems.Queries.GetProblemsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Search.Discussions.Problems;

/// <summary>
/// Handles search operations for discussion problems.
/// </summary>
[Route("api/v{version:apiVersion}/search/problems")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProblemsSearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProblemsSearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Searches discussion problems based on keywords, date range, difficulty level, resolution status, and pagination.
    /// </summary>
    /// <param name="query">The problem search query parameters.</param>
    /// <response code="200">Returns paginated problem summary search results <see cref="SearchResult{ProblemSummaryDto}"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<SearchResult<ProblemSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<SearchResult<ProblemSummaryDto>>>> SearchProblemsAsync([FromQuery] GetProblemsSearchQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}

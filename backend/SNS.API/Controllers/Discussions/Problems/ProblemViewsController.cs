using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Discussions.Problems.ProblemViews.Commands.RecordProblemView;
using SNS.Application.Discussions.Problems.ProblemViews.Contracts;
using SNS.Application.Discussions.Problems.ProblemViews.Queries.GetProblemViewers;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Discussions.Problems;

/// <summary>
/// Handles view interactions and view analytics queries for discussion problems.
/// </summary>
[Route("api/v{version:apiVersion}/problems/{problemId:guid}/views")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProblemViewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProblemViewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Records a view interaction on a discussion problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <param name="request">View metadata payload.</param>
    /// <response code="200">View recorded.</response>
    /// <response code="404">Problem not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> RecordProblemViewAsync(
        [FromRoute] Guid problemId,
        [FromBody] RecordProblemViewCommand request)
    {
        var command = request with { ProblemId = problemId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of viewers for a problem (author-only query).
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page number.</param>
    /// <param name="searchTerm">Optional search keyword.</param>
    /// <response code="200">Viewer profiles retrieved.</response>
    /// <response code="403">Current user is not the author.</response>
    /// <response code="404">Problem not found.</response>
    [HttpGet("viewers")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<ProblemViewerDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<ProblemViewerDto>>>> GetProblemViewersAsync(
        [FromRoute] Guid problemId,
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1,
        [FromQuery] string? searchTerm = null)
    {
        return (await _mediator.Send(new GetProblemViewersQuery(problemId, pageSize, currentPage, searchTerm))).ToActionResult(this);
    }
}

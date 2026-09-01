using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Discussions.Problems.ProblemTopics.Contracts;
using SNS.Application.Discussions.Problems.ProblemTopics.Queries.GetProblemTopics;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Discussions.Problems;

/// <summary>
/// Handles querying AI-classified topics for discussion problems.
/// </summary>
[Route("api/v{version:apiVersion}/problems/{problemId:guid}/topics")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProblemTopicsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProblemTopicsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves AI-detected topics and confidence scores for a discussion problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <response code="200">Topics retrieved.</response>
    /// <response code="404">Problem not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<ProblemTopicDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<List<ProblemTopicDto>>>> GetProblemTopicsAsync([FromRoute] Guid problemId)
    {
        return (await _mediator.Send(new GetProblemTopicsQuery(problemId))).ToActionResult(this);
    }
}

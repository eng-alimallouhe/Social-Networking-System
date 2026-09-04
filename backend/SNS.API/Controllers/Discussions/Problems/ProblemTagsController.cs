using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Discussions.Problems.ProblemTags.Commands.AddProblemTag;
using SNS.Application.Discussions.Problems.ProblemTags.Commands.RemoveProblemTag;
using SNS.Application.Discussions.Problems.ProblemTags.Contracts;
using SNS.Application.Discussions.Problems.ProblemTags.Queries.GetProblemTags;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Discussions.Problems;

/// <summary>
/// Handles tag management and associations for discussion problems.
/// </summary>
[Route("api/v{version:apiVersion}/problems/{problemId:guid}/tags")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProblemTagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProblemTagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Attaches a tag to a discussion problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <param name="request">Tag addition payload.</param>
    /// <response code="201">Tag attached.</response>
    /// <response code="400">Tag already exists or invalid input.</response>
    /// <response code="403">Current user does not own the problem.</response>
    /// <response code="404">Problem not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> AddProblemTagAsync(
        [FromRoute] Guid problemId,
        [FromBody] AddProblemTagCommand request)
    {
        var command = request with { ProblemId = problemId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Removes an existing tag association from a discussion problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <param name="tagId">The tag unique identifier to remove.</param>
    /// <response code="200">Tag association removed.</response>
    /// <response code="403">Current user does not own the problem.</response>
    /// <response code="404">Problem or tag association not found.</response>
    [HttpDelete("{tagId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> RemoveProblemTagAsync(
        [FromRoute] Guid problemId,
        [FromRoute] Guid tagId)
    {
        return (await _mediator.Send(new RemoveProblemTagCommand(problemId, tagId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves all tags associated with a discussion problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <response code="200">Tags list retrieved.</response>
    /// <response code="404">Problem not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<ProblemTagDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<List<ProblemTagDto>>>> GetProblemTagsAsync([FromRoute] Guid problemId)
    {
        return (await _mediator.Send(new GetProblemTagsQuery(problemId))).ToActionResult(this);
    }
}

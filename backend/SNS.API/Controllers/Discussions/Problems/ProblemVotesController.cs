using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Discussions.Problems.ProblemVotes.Commands.AddOrChangeProblemVote;
using SNS.Application.Discussions.Problems.ProblemVotes.Commands.RemoveProblemVote;
using SNS.Application.Discussions.Problems.ProblemVotes.Contracts;
using SNS.Application.Discussions.Problems.ProblemVotes.Queries.GetMyProblemVote;
using SNS.Application.Discussions.Problems.ProblemVotes.Queries.GetProblemVoteSummary;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Discussions.Problems;

/// <summary>
/// Handles voting operations and metrics for discussion problems.
/// </summary>
[Route("api/v{version:apiVersion}/problems/{problemId:guid}/votes")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProblemVotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProblemVotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Casts or updates a vote (Upvote / Downvote) on a discussion problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <param name="request">The vote request containing the vote type.</param>
    /// <response code="200">Vote applied or already applied.</response>
    /// <response code="201">Vote created.</response>
    /// <response code="404">Problem not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> AddOrChangeProblemVoteAsync(
        [FromRoute] Guid problemId,
        [FromBody] AddOrChangeProblemVoteCommand request)
    {
        var command = request with { ProblemId = problemId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Removes the current user's vote from a discussion problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <response code="200">Vote successfully removed.</response>
    /// <response code="404">Problem not found.</response>
    [HttpDelete]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> RemoveProblemVoteAsync([FromRoute] Guid problemId)
    {
        return (await _mediator.Send(new RemoveProblemVoteCommand(problemId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves aggregate voting statistics and current user vote status for a problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <response code="200">Voting summary metrics retrieved.</response>
    /// <response code="404">Problem not found.</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(Result<ProblemVoteSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ProblemVoteSummaryDto>>> GetProblemVoteSummaryAsync([FromRoute] Guid problemId)
    {
        return (await _mediator.Send(new GetProblemVoteSummaryQuery(problemId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves the current authenticated user's vote on a problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <response code="200">User's vote status retrieved.</response>
    [HttpGet("my-vote")]
    [Authorize]
    [ProducesResponseType(typeof(Result<VoteType?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<VoteType?>>> GetMyProblemVoteAsync([FromRoute] Guid problemId)
    {
        return (await _mediator.Send(new GetMyProblemVoteQuery(problemId))).ToActionResult(this);
    }
}

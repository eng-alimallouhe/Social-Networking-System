using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Discussions.Solutions.SolutionVotes.Commands.AddOrChangeSolutionVote;
using SNS.Application.Discussions.Solutions.SolutionVotes.Commands.RemoveSolutionVote;
using SNS.Application.Discussions.Solutions.SolutionVotes.Contracts;
using SNS.Application.Discussions.Solutions.SolutionVotes.Queries.GetMySolutionVote;
using SNS.Application.Discussions.Solutions.SolutionVotes.Queries.GetSolutionVoteSummary;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Discussions.Solutions;

/// <summary>
/// Handles voting operations and metrics for proposed solutions.
/// </summary>
[Route("api/v{version:apiVersion}/solutions/{solutionId:guid}/votes")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class SolutionVotesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SolutionVotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Casts or updates a vote (Upvote / Downvote) on a proposed solution.
    /// </summary>
    /// <param name="solutionId">The solution unique identifier.</param>
    /// <param name="request">The vote request containing the vote type.</param>
    /// <response code="200">Vote applied or already applied.</response>
    /// <response code="201">Vote created.</response>
    /// <response code="404">Solution not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> AddOrChangeSolutionVoteAsync(
        [FromRoute] Guid solutionId,
        [FromBody] AddOrChangeSolutionVoteCommand request)
    {
        var command = request with { SolutionId = solutionId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Removes the current user's vote from a proposed solution.
    /// </summary>
    /// <param name="solutionId">The solution unique identifier.</param>
    /// <response code="200">Vote successfully removed.</response>
    /// <response code="404">Solution not found.</response>
    [HttpDelete]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> RemoveSolutionVoteAsync([FromRoute] Guid solutionId)
    {
        return (await _mediator.Send(new RemoveSolutionVoteCommand(solutionId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves aggregate voting statistics and current user vote status for a solution.
    /// </summary>
    /// <param name="solutionId">The solution unique identifier.</param>
    /// <response code="200">Voting summary metrics retrieved.</response>
    /// <response code="404">Solution not found.</response>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(Result<SolutionVoteSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<SolutionVoteSummaryDto>>> GetSolutionVoteSummaryAsync([FromRoute] Guid solutionId)
    {
        return (await _mediator.Send(new GetSolutionVoteSummaryQuery(solutionId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves the current authenticated user's vote on a solution.
    /// </summary>
    /// <param name="solutionId">The solution unique identifier.</param>
    /// <response code="200">User's vote status retrieved.</response>
    [HttpGet("my-vote")]
    [Authorize]
    [ProducesResponseType(typeof(Result<VoteType?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<VoteType?>>> GetMySolutionVoteAsync([FromRoute] Guid solutionId)
    {
        return (await _mediator.Send(new GetMySolutionVoteQuery(solutionId))).ToActionResult(this);
    }
}

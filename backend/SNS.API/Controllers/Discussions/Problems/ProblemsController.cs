using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Discussions.Problems.Problems.Commands.ChangeProblemStatus;
using SNS.Application.Discussions.Problems.Problems.Commands.CreateProblem;
using SNS.Application.Discussions.Problems.Problems.Commands.DeleteProblem;
using SNS.Application.Discussions.Problems.Problems.Commands.UpdateProblem;
using SNS.Application.Discussions.Problems.Problems.Contracts;
using SNS.Application.Discussions.Problems.Problems.Queries.GetMyProblems;
using SNS.Application.Discussions.Problems.Problems.Queries.GetProblemById;
using SNS.Application.Discussions.Problems.Problems.Queries.GetProblemsByAuthor;
using SNS.Application.Discussions.Problems.Problems.Queries.GetProblemsByCommunity;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Discussions.Problems;

/// <summary>
/// Handles core CRUD operations, lifecycle status management, and queries for discussion problems.
/// </summary>
[Route("api/v{version:apiVersion}/problems")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProblemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProblemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new discussion problem.
    /// </summary>
    /// <param name="request">Problem creation details.</param>
    /// <response code="201">Problem successfully created.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Guid>>> CreateProblemAsync([FromBody] CreateProblemCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves full details of a discussion problem by its ID.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <response code="200">Problem details retrieved.</response>
    /// <response code="404">Problem not found.</response>
    [HttpGet("{problemId:guid}")]
    [ProducesResponseType(typeof(Result<ProblemDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ProblemDetailsDto>>> GetProblemByIdAsync([FromRoute] Guid problemId)
    {
        return (await _mediator.Send(new GetProblemByIdQuery(problemId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of discussion problems authored by the current authenticated user.
    /// </summary>
    /// <param name="query">Pagination and search parameters.</param>
    /// <response code="200">User's problems retrieved.</response>
    [HttpGet("my-problems")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<ProblemSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Paged<ProblemSummaryDto>>>> GetMyProblemsAsync([FromQuery] GetMyProblemsQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of discussion problems authored by a specific profile.
    /// </summary>
    /// <param name="authorId">The profile ID of the author.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page number.</param>
    /// <param name="searchTerm">Optional search keyword.</param>
    /// <response code="200">Author's problems retrieved.</response>
    [HttpGet("author/{authorId:guid}")]
    [ProducesResponseType(typeof(Result<Paged<ProblemSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<ProblemSummaryDto>>>> GetProblemsByAuthorAsync(
        [FromRoute] Guid authorId,
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1,
        [FromQuery] string? searchTerm = null)
    {
        return (await _mediator.Send(new GetProblemsByAuthorQuery(authorId, pageSize, currentPage, searchTerm))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of discussion problems posted in a community.
    /// </summary>
    /// <param name="communityId">The community unique identifier.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page number.</param>
    /// <param name="searchTerm">Optional search keyword.</param>
    /// <response code="200">Community problems retrieved.</response>
    [HttpGet("community/{communityId:guid}")]
    [ProducesResponseType(typeof(Result<Paged<ProblemSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<ProblemSummaryDto>>>> GetProblemsByCommunityAsync(
        [FromRoute] Guid communityId,
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1,
        [FromQuery] string? searchTerm = null)
    {
        return (await _mediator.Send(new GetProblemsByCommunityQuery(communityId, pageSize, currentPage, searchTerm))).ToActionResult(this);
    }

    /// <summary>
    /// Updates metadata and structured content blocks of an existing problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <param name="request">Updated problem payload.</param>
    /// <response code="200">Problem successfully updated.</response>
    /// <response code="403">User does not own the problem.</response>
    /// <response code="404">Problem not found.</response>
    [HttpPut("{problemId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UpdateProblemAsync(
        [FromRoute] Guid problemId,
        [FromBody] UpdateProblemCommand request)
    {
        var command = request with { ProblemId = problemId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Soft-deletes an existing discussion problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <response code="200">Problem successfully deleted.</response>
    /// <response code="403">User does not own the problem.</response>
    /// <response code="404">Problem not found.</response>
    [HttpDelete("{problemId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> DeleteProblemAsync([FromRoute] Guid problemId)
    {
        return (await _mediator.Send(new DeleteProblemCommand(problemId))).ToActionResult(this);
    }

    /// <summary>
    /// Updates the lifecycle status of a discussion problem (Open, Solved, Closed).
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <param name="request">Status change payload.</param>
    /// <response code="200">Status successfully changed.</response>
    /// <response code="403">User does not own the problem.</response>
    /// <response code="404">Problem not found.</response>
    [HttpPatch("{problemId:guid}/status")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> ChangeProblemStatusAsync(
        [FromRoute] Guid problemId,
        [FromBody] ChangeProblemStatusCommand request)
    {
        var command = request with { ProblemId = problemId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }
}

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Discussions.Solutions.Solutions.Commands.ChangeSolutionStatus;
using SNS.Application.Discussions.Solutions.Solutions.Commands.CreateSolution;
using SNS.Application.Discussions.Solutions.Solutions.Commands.DeleteSolution;
using SNS.Application.Discussions.Solutions.Solutions.Commands.UpdateSolution;
using SNS.Application.Discussions.Solutions.Solutions.Contracts;
using SNS.Application.Discussions.Solutions.Solutions.Queries.GetMySolutions;
using SNS.Application.Discussions.Solutions.Solutions.Queries.GetProblemSolutions;
using SNS.Application.Discussions.Solutions.Solutions.Queries.GetSolutionById;
using SNS.Application.Discussions.Solutions.Solutions.Queries.GetSolutionsByAuthor;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Discussions.Solutions;

/// <summary>
/// Handles CRUD operations, lifecycle status transitions, and query retrieval for solutions to discussion problems.
/// </summary>
[Route("api/v{version:apiVersion}/solutions")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class SolutionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SolutionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Submits a new proposed solution for a discussion problem.
    /// </summary>
    /// <param name="request">Solution submission payload.</param>
    /// <response code="201">Solution successfully created.</response>
    /// <response code="400">Problem is closed or input is invalid.</response>
    /// <response code="404">Problem not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<Guid>>> CreateSolutionAsync([FromBody] CreateSolutionCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves complete details of a solution by its unique identifier.
    /// </summary>
    /// <param name="solutionId">The solution unique identifier.</param>
    /// <response code="200">Solution details retrieved.</response>
    /// <response code="404">Solution not found.</response>
    [HttpGet("{solutionId:guid}")]
    [ProducesResponseType(typeof(Result<SolutionDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<SolutionDetailsDto>>> GetSolutionByIdAsync([FromRoute] Guid solutionId)
    {
        return (await _mediator.Send(new GetSolutionByIdQuery(solutionId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of solutions submitted by the authenticated user.
    /// </summary>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page number.</param>
    /// <response code="200">Solutions retrieved.</response>
    [HttpGet("my-solutions")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<SolutionSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<SolutionSummaryDto>>>> GetMySolutionsAsync(
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1)
    {
        return (await _mediator.Send(new GetMySolutionsQuery(pageSize, currentPage))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of solutions submitted by a specific author profile.
    /// </summary>
    /// <param name="authorId">The profile ID of the author.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page number.</param>
    /// <response code="200">Solutions retrieved.</response>
    [HttpGet("author/{authorId:guid}")]
    [ProducesResponseType(typeof(Result<Paged<SolutionSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<SolutionSummaryDto>>>> GetSolutionsByAuthorAsync(
        [FromRoute] Guid authorId,
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1)
    {
        return (await _mediator.Send(new GetSolutionsByAuthorQuery(authorId, pageSize, currentPage))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of solutions proposed for a specific discussion problem.
    /// </summary>
    /// <param name="problemId">The problem unique identifier.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page number.</param>
    /// <response code="200">Problem solutions retrieved.</response>
    /// <response code="404">Problem not found.</response>
    [HttpGet("~/api/v{version:apiVersion}/problems/{problemId:guid}/solutions")]
    [ProducesResponseType(typeof(Result<Paged<SolutionSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<Paged<SolutionSummaryDto>>>> GetProblemSolutionsAsync(
        [FromRoute] Guid problemId,
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1)
    {
        return (await _mediator.Send(new GetProblemSolutionsQuery(problemId, pageSize, currentPage))).ToActionResult(this);
    }

    /// <summary>
    /// Updates the structured content blocks of an existing solution.
    /// </summary>
    /// <param name="solutionId">The solution unique identifier.</param>
    /// <param name="request">The updated solution content.</param>
    /// <response code="200">Solution successfully updated.</response>
    /// <response code="403">Current user does not own the solution.</response>
    /// <response code="404">Solution not found.</response>
    [HttpPut("{solutionId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateSolutionAsync(
        [FromRoute] Guid solutionId,
        [FromBody] UpdateSolutionCommand request)
    {
        var command = request with { SolutionId = solutionId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Soft-deletes an existing solution.
    /// </summary>
    /// <param name="solutionId">The solution unique identifier.</param>
    /// <response code="200">Solution successfully deleted.</response>
    /// <response code="403">Current user does not own the solution.</response>
    /// <response code="404">Solution not found.</response>
    [HttpDelete("{solutionId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> DeleteSolutionAsync([FromRoute] Guid solutionId)
    {
        return (await _mediator.Send(new DeleteSolutionCommand(solutionId))).ToActionResult(this);
    }

    /// <summary>
    /// Updates the lifecycle status of a solution (e.g. Accepted, BestSolution, Rejected).
    /// </summary>
    /// <param name="solutionId">The solution unique identifier.</param>
    /// <param name="request">Status change payload.</param>
    /// <response code="200">Status successfully updated.</response>
    /// <response code="403">User is not authorized to update solution status.</response>
    /// <response code="404">Solution or problem not found.</response>
    [HttpPatch("{solutionId:guid}/status")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> ChangeSolutionStatusAsync(
        [FromRoute] Guid solutionId,
        [FromBody] ChangeSolutionStatusCommand request)
    {
        var command = request with { SolutionId = solutionId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }
}

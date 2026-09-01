using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Resumes.Resumes.Commands.CreateResume;
using SNS.Application.Resumes.Resumes.Commands.DeleteResume;
using SNS.Application.Resumes.Resumes.Commands.UpdateResume;
using SNS.Application.Resumes.Resumes.Contracts;
using SNS.Application.Resumes.Resumes.Queries.GetMyResumes;
using SNS.Application.Resumes.Resumes.Queries.GetResumeById;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Resumes.Resumes;

/// <summary>
/// Handles root resume aggregate operations including creation, retrieval, updates, and deletion.
/// </summary>
[Route("api/v{version:apiVersion}/resumes")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ResumesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new resume for the authenticated user.
    /// </summary>
    /// <param name="request">The resume creation payload.</param>
    /// <response code="201">Returns the identifier of the newly created resume.</response>
    /// <response code="400">The provided resume data is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Guid>>> CreateResumeAsync([FromBody] CreateResumeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves all active resumes owned by the authenticated user.
    /// </summary>
    /// <response code="200">Returns the list of resume summary cards.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("my-resumes")]
    [ProducesResponseType(typeof(Result<List<ResumeSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<List<ResumeSummaryDto>>>> GetMyResumesAsync()
    {
        return (await _mediator.Send(new GetMyResumesQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves the complete details of a resume by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the resume.</param>
    /// <response code="200">Returns the full resume details including all sections.</response>
    /// <response code="404">The resume was not found or is inactive.</response>
    [MapToApiVersion("1.0")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<ResumeDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ResumeDetailsDto>>> GetResumeByIdAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new GetResumeByIdQuery(id))).ToActionResult(this);
    }

    /// <summary>
    /// Updates the core information, template, summary, and language of a resume.
    /// </summary>
    /// <param name="id">The unique identifier of the resume to update.</param>
    /// <param name="request">The updated resume payload.</param>
    /// <response code="200">The resume was updated successfully.</response>
    /// <response code="400">The provided update payload is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The authenticated user is not the owner of the resume.</response>
    /// <response code="404">The resume was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UpdateResumeAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateResumeCommand request)
    {
        var command = request with { ResumeId = id };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Soft-deletes a resume owned by the authenticated user.
    /// </summary>
    /// <param name="id">The unique identifier of the resume to delete.</param>
    /// <response code="200">The resume was deleted successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The authenticated user is not the owner of the resume.</response>
    /// <response code="404">The resume was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> DeleteResumeAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new DeleteResumeCommand(id))).ToActionResult(this);
    }
}

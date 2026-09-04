using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Resumes.Educations.Commands.AddResumeEducation;
using SNS.Application.Resumes.Educations.Commands.DeleteResumeEducation;
using SNS.Application.Resumes.Educations.Commands.UpdateResumeEducation;
using SNS.Application.Resumes.Educations.Contracts;
using SNS.Application.Resumes.Educations.Queries.GetResumeEducations;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Resumes.Educations;

/// <summary>
/// Handles educational history operations for a resume.
/// </summary>
[Route("api/v{version:apiVersion}/resumes/{resumeId:guid}/educations")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ResumeEducationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeEducationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all education history entries for a specific resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <response code="200">Returns the list of education history records.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<ResumeEducationDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<ResumeEducationDto>>>> GetResumeEducationsAsync([FromRoute] Guid resumeId)
    {
        return (await _mediator.Send(new GetResumeEducationsQuery(resumeId))).ToActionResult(this);
    }

    /// <summary>
    /// Adds a new education entry to a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="request">The education creation payload.</param>
    /// <response code="201">Returns the identifier of the created education record.</response>
    /// <response code="400">The provided education data is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<Guid>>> AddResumeEducationAsync(
        [FromRoute] Guid resumeId,
        [FromBody] AddResumeEducationCommand request)
    {
        var command = request with { ResumeId = resumeId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates an existing education entry on a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="educationId">The unique identifier of the education entry to update.</param>
    /// <param name="request">The updated education payload.</param>
    /// <response code="200">The education entry was updated successfully.</response>
    /// <response code="400">The provided payload is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or education entry was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{educationId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateResumeEducationAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid educationId,
        [FromBody] UpdateResumeEducationCommand request)
    {
        var command = request with { ResumeId = resumeId, EducationId = educationId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Deletes an education entry from a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="educationId">The unique identifier of the education entry to delete.</param>
    /// <response code="200">The education entry was deleted successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or education entry was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{educationId:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> DeleteResumeEducationAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid educationId)
    {
        return (await _mediator.Send(new DeleteResumeEducationCommand(resumeId, educationId))).ToActionResult(this);
    }
}

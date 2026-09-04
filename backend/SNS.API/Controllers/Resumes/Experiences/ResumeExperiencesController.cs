using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Resumes.Experiences.Commands.AddResumeExperience;
using SNS.Application.Resumes.Experiences.Commands.DeleteResumeExperience;
using SNS.Application.Resumes.Experiences.Commands.UpdateResumeExperience;
using SNS.Application.Resumes.Experiences.Contracts;
using SNS.Application.Resumes.Experiences.Queries.GetResumeExperiences;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Resumes.Experiences;

/// <summary>
/// Handles professional work experience history operations for a resume.
/// </summary>
[Route("api/v{version:apiVersion}/resumes/{resumeId:guid}/experiences")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ResumeExperiencesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeExperiencesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all work experience entries for a specific resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <response code="200">Returns the list of work experience records.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<ResumeExperienceDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<ResumeExperienceDto>>>> GetResumeExperiencesAsync([FromRoute] Guid resumeId)
    {
        return (await _mediator.Send(new GetResumeExperiencesQuery(resumeId))).ToActionResult(this);
    }

    /// <summary>
    /// Adds a new work experience entry to a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="request">The experience creation payload.</param>
    /// <response code="201">Returns the identifier of the created experience record.</response>
    /// <response code="400">The provided experience data is invalid.</response>
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
    public async Task<ActionResult<Result<Guid>>> AddResumeExperienceAsync(
        [FromRoute] Guid resumeId,
        [FromBody] AddResumeExperienceCommand request)
    {
        var command = request with { ResumeId = resumeId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates an existing work experience entry on a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="experienceId">The unique identifier of the experience entry to update.</param>
    /// <param name="request">The updated experience payload.</param>
    /// <response code="200">The experience entry was updated successfully.</response>
    /// <response code="400">The provided payload is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or experience entry was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{experienceId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateResumeExperienceAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid experienceId,
        [FromBody] UpdateResumeExperienceCommand request)
    {
        var command = request with { ResumeId = resumeId, ExperienceId = experienceId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Deletes a work experience entry from a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="experienceId">The unique identifier of the experience entry to delete.</param>
    /// <response code="200">The experience entry was deleted successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or experience entry was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{experienceId:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> DeleteResumeExperienceAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid experienceId)
    {
        return (await _mediator.Send(new DeleteResumeExperienceCommand(resumeId, experienceId))).ToActionResult(this);
    }
}

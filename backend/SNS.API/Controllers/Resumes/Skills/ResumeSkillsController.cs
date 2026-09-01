using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Resumes.Skills.Commands.AddResumeSkill;
using SNS.Application.Resumes.Skills.Commands.DeleteResumeSkill;
using SNS.Application.Resumes.Skills.Commands.UpdateResumeSkill;
using SNS.Application.Resumes.Skills.Contracts;
using SNS.Application.Resumes.Skills.Queries.GetResumeSkills;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Resumes.Skills;

/// <summary>
/// Handles skill entries for a resume.
/// </summary>
[Route("api/v{version:apiVersion}/resumes/{resumeId:guid}/skills")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ResumeSkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeSkillsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all skill entries for a specific resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <response code="200">Returns the list of skill records.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<ResumeSkillDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<ResumeSkillDto>>>> GetResumeSkillsAsync([FromRoute] Guid resumeId)
    {
        return (await _mediator.Send(new GetResumeSkillsQuery(resumeId))).ToActionResult(this);
    }

    /// <summary>
    /// Adds a new skill entry to a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="request">The skill creation payload.</param>
    /// <response code="201">Returns the identifier of the created skill record.</response>
    /// <response code="400">The provided skill data is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume was not found.</response>
    /// <response code="409">The skill already exists on this resume.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<Guid>>> AddResumeSkillAsync(
        [FromRoute] Guid resumeId,
        [FromBody] AddResumeSkillCommand request)
    {
        var command = request with { ResumeId = resumeId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates an existing skill entry on a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="skillId">The unique identifier of the skill entry to update.</param>
    /// <param name="request">The updated skill payload.</param>
    /// <response code="200">The skill entry was updated successfully.</response>
    /// <response code="400">The provided payload is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or skill entry was not found.</response>
    /// <response code="409">A skill with the updated name already exists on this resume.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{skillId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result>> UpdateResumeSkillAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid skillId,
        [FromBody] UpdateResumeSkillCommand request)
    {
        var command = request with { ResumeId = resumeId, SkillId = skillId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Deletes a skill entry from a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="skillId">The unique identifier of the skill entry to delete.</param>
    /// <response code="200">The skill entry was deleted successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or skill entry was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{skillId:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> DeleteResumeSkillAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid skillId)
    {
        return (await _mediator.Send(new DeleteResumeSkillCommand(resumeId, skillId))).ToActionResult(this);
    }
}

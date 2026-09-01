using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Resumes.Projects.Commands.AddResumeProject;
using SNS.Application.Resumes.Projects.Commands.RemoveResumeProject;
using SNS.Application.Resumes.Projects.Contracts;
using SNS.Application.Resumes.Projects.Queries.GetResumeProjects;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Resumes.Projects;

/// <summary>
/// Handles project showcase associations for a resume.
/// </summary>
[Route("api/v{version:apiVersion}/resumes/{resumeId:guid}/projects")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ResumeProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all showcase projects linked to a specific resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <response code="200">Returns the list of linked project summaries.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<ResumeProjectDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<ResumeProjectDto>>>> GetResumeProjectsAsync([FromRoute] Guid resumeId)
    {
        return (await _mediator.Send(new GetResumeProjectsQuery(resumeId))).ToActionResult(this);
    }

    /// <summary>
    /// Links an existing project to a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="request">The project link command payload containing ProjectId.</param>
    /// <response code="200">The project was successfully linked to the resume.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or target project was not found.</response>
    /// <response code="409">The project is already associated with this resume.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result>> AddResumeProjectAsync(
        [FromRoute] Guid resumeId,
        [FromBody] AddResumeProjectCommand request)
    {
        var command = request with { ResumeId = resumeId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Unlinks a project from a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="projectId">The unique identifier of the project to unlink.</param>
    /// <response code="200">The project link was removed successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or project link was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{projectId:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> RemoveResumeProjectAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid projectId)
    {
        return (await _mediator.Send(new RemoveResumeProjectCommand(resumeId, projectId))).ToActionResult(this);
    }
}

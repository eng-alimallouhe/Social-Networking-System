using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Jobs.JobSkills.Commands.AddJobSkill;
using SNS.Application.Jobs.JobSkills.Commands.RemoveJobSkill;
using SNS.Application.Jobs.JobSkills.Contracts;
using SNS.Application.Jobs.JobSkills.Queries.GetJobSkills;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Jobs.JobSkills;

/// <summary>
/// Request payload for associating a skill with a job posting.
/// </summary>
/// <param name="SkillId">The unique identifier of the skill.</param>
public sealed record AddJobSkillRequest(Guid SkillId);

/// <summary>
/// Handles querying, adding, and removing required/preferred skills associated with job postings.
/// </summary>
[Route("api/v{version:apiVersion}/jobs/{jobId:guid}/skills")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class JobSkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobSkillsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all skills associated with a specific job posting.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <response code="200">Skills retrieved successfully.</response>
    /// <response code="404">Job not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<JobSkillDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<List<JobSkillDto>>>> GetJobSkillsAsync([FromRoute] Guid jobId)
    {
        return (await _mediator.Send(new GetJobSkillsQuery(jobId))).ToActionResult(this);
    }

    /// <summary>
    /// Associates a skill with a job posting.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="request">Skill association payload.</param>
    /// <response code="201">Skill successfully added.</response>
    /// <response code="400">Skill is already associated with this job.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not a company administrator.</response>
    /// <response code="404">Job or skill not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<Guid>>> AddJobSkillAsync(
        [FromRoute] Guid jobId,
        [FromBody] AddJobSkillRequest request)
    {
        var command = new AddJobSkillCommand(jobId, request.SkillId);
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Removes a skill association from a job posting.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="skillId">The unique identifier of the skill.</param>
    /// <response code="200">Skill successfully removed.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not a company administrator.</response>
    /// <response code="404">Job or skill association not found.</response>
    [HttpDelete("{skillId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> RemoveJobSkillAsync(
        [FromRoute] Guid jobId,
        [FromRoute] Guid skillId)
    {
        return (await _mediator.Send(new RemoveJobSkillCommand(jobId, skillId))).ToActionResult(this);
    }
}

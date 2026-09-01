using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Projects.Commands.Create.AddProjectMilestone;
using SNS.Application.Projects.Commands.Delete.DeleteProjectMilestone;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Projects;

[Route("api/v{version:apiVersion}/projects/{projectId:guid}/milestones")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProjectMilestonesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectMilestonesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> AddProjectMilestoneAsync(
        [FromRoute] Guid projectId,
        [FromBody] AddProjectMilestoneCommand command)
    {
        if (projectId != command.ProjectId) return BadRequest(Result.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{milestoneId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> DeleteProjectMilestoneAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid milestoneId)
    {
        return (await _mediator.Send(new DeleteProjectMilestoneCommand(projectId, milestoneId))).ToActionResult(this);
    }
}

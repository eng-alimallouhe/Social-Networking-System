using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Projects.Commands.Create.AddProjectSkill;
using SNS.Application.Projects.Commands.Delete.RemoveProjectSkill;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Projects;

[Route("api/v{version:apiVersion}/projects/{projectId:guid}/skills")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProjectSkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectSkillsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> AddProjectSkillAsync(
        [FromRoute] Guid projectId,
        [FromBody] AddProjectSkillCommand command)
    {
        if (projectId != command.ProjectId) return BadRequest(Result.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{projectSkillId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> RemoveProjectSkillAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid projectSkillId)
    {
        return (await _mediator.Send(new RemoveProjectSkillCommand(projectId, projectSkillId))).ToActionResult(this);
    }
}

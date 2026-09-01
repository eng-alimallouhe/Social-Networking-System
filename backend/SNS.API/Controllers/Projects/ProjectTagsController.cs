using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Projects.Commands.Create.AddProjectTag;
using SNS.Application.Projects.Commands.Delete.RemoveProjectTag;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Projects;

[Route("api/v{version:apiVersion}/projects/{projectId:guid}/tags")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProjectTagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectTagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> AddProjectTagAsync(
        [FromRoute] Guid projectId,
        [FromBody] AddProjectTagCommand command)
    {
        if (projectId != command.ProjectId) return BadRequest(Result.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{projectTagId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> RemoveProjectTagAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid projectTagId)
    {
        return (await _mediator.Send(new RemoveProjectTagCommand(projectId, projectTagId))).ToActionResult(this);
    }
}

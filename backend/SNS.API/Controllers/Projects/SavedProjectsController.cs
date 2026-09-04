using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Projects.Commands.Interaction.SaveProject;
using SNS.Application.Projects.Commands.Interaction.UnsaveProject;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Projects;

[Route("api/v{version:apiVersion}/projects/{projectId:guid}/save")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class SavedProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SavedProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> SaveProjectAsync([FromRoute] Guid projectId)
    {
        return (await _mediator.Send(new SaveProjectCommand(projectId))).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> UnsaveProjectAsync([FromRoute] Guid projectId)
    {
        return (await _mediator.Send(new UnsaveProjectCommand(projectId))).ToActionResult(this);
    }
}

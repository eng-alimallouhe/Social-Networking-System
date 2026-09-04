using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Projects.Commands.Interaction.RecordProjectView;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Projects;

[Route("api/v{version:apiVersion}/projects/{projectId:guid}/views")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProjectViewsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectViewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> RecordProjectViewAsync([FromRoute] Guid projectId)
    {
        return (await _mediator.Send(new RecordProjectViewCommand(projectId))).ToActionResult(this);
    }
}

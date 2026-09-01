using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Projects.Commands.Update.RateProject;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Projects;

[Route("api/v{version:apiVersion}/projects/{projectId:guid}/ratings")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProjectRatingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectRatingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> RateProjectAsync(
        [FromRoute] Guid projectId,
        [FromBody] RateProjectCommand command)
    {
        if (projectId != command.ProjectId) return BadRequest(Result.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        return (await _mediator.Send(command)).ToActionResult(this);
    }
}

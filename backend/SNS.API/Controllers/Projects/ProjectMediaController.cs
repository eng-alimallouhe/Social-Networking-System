using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Attributes;
using SNS.API.Extensions;
using SNS.Application.Projects.Commands.Create.AddProjectMedia;
using SNS.Application.Projects.Commands.Delete.DeleteProjectMedia;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Projects;

[Route("api/v{version:apiVersion}/projects/{projectId:guid}/media")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProjectMediaController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectMediaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> AddProjectMediaAsync(
        [FromRoute] Guid projectId,
        [FromForm] IFormFile file,
        [FromForm] string caption,
        [FromForm] MediaType type)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(Result.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        }

        using var stream = file.OpenReadStream();
        var command = new AddProjectMediaCommand(
            ProjectId: projectId,
            FileStream: stream,
            ContentType: file.ContentType,
            FileName: file.FileName,
            Caption: caption,
            Type: type
        );

        return (await _mediator.Send(command)).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{mediaId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> DeleteProjectMediaAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid mediaId)
    {
        return (await _mediator.Send(new DeleteProjectMediaCommand(projectId, mediaId))).ToActionResult(this);
    }
}

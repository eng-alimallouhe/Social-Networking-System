using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Projects.Commands.Create.AddProjectContributor;
using SNS.Application.Projects.Commands.Delete.RemoveProjectContributor;
using SNS.Application.Projects.Commands.Update.ChangeContributorRequestStatus;
using SNS.Application.Projects.Contracts;
using SNS.Application.Projects.Queries.GetProjectParticipantsForOwner;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Projects;

public record ChangeContributorStatusRequest(bool IsAccepted);

[Route("api/v{version:apiVersion}/projects/{projectId:guid}/contributors")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProjectContributorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectContributorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("management")]
    [ProducesResponseType(typeof(Paged<ProjectContributorManagementDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<ProjectContributorManagementDto>>>> GetProjectParticipantsForOwnerAsync(
        [FromRoute] Guid projectId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return (await _mediator.Send(new GetProjectParticipantsForOwnerQuery(projectId, page, pageSize))).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> AddProjectContributorAsync(
        [FromRoute] Guid projectId,
        [FromBody] AddProjectContributorCommand command)
    {
        if (projectId != command.ProjectId) return BadRequest(Result.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> ChangeContributorRequestStatusAsync(
        [FromRoute] Guid projectId,
        [FromBody] ChangeContributorStatusRequest request)
    {
        return (await _mediator.Send(new ChangeContributorRequestStatusCommand(projectId, request.IsAccepted))).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{contributorId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> RemoveProjectContributorAsync(
        [FromRoute] Guid projectId,
        [FromRoute] Guid contributorId)
    {
        return (await _mediator.Send(new RemoveProjectContributorCommand(projectId, contributorId))).ToActionResult(this);
    }
}
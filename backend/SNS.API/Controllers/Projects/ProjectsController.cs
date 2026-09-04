using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SNS.API.Extensions;
using SNS.Application.Projects.Contracts;
using SNS.Application.Projects.Queries.GetProjectById;
using SNS.Application.Projects.Queries.GetProjectFeed;
using SNS.Application.Projects.Queries.GetProjectMedia;
using SNS.Application.Projects.Queries.GetProjectParticipants;
using SNS.Application.Projects.Queries.GetProjectRatings;
using SNS.Application.Projects.Queries.GetProjectMilestones;
using SNS.Application.Projects.Commands.Create.CreateProject;
using SNS.Application.Projects.Commands.Update.ChangeProjectStatus;
using SNS.Application.Projects.Commands.Update.UpdateProjectBasicInfo;
using SNS.Application.Projects.Commands.Update.UpdateProject;
using SNS.Application.Projects.Commands.Update.UpdateProjectReadme;
using SNS.Application.Projects.Queries.GetProjectSourceCode;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Projects.ValueObjects;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Projects;

/// <summary>
/// Handles project retrieval and core management operations.
/// </summary>
[Route("api/v{version:apiVersion}/projects")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a personalized feed of projects for the authenticated user.
    /// </summary>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("feed")]
    [ProducesResponseType(typeof(List<ProjectOverviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<List<ProjectOverviewDto>>>> GetProjectFeedAsync(
        [FromQuery] int CurrentPage = 1,
        [FromQuery] int PageSize = 10)
    {
        return (await _mediator.Send(new GetProjectFeedQuery(CurrentPage, PageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a single project by its ID, including its details.
    /// </summary>
    [MapToApiVersion("1.0")]
    [HttpGet("{projectId:guid}")]
    [ProducesResponseType(typeof(ProjectDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ProjectDetailsDto>>> GetProjectByIdAsync([FromRoute] Guid projectId)
    {
        return (await _mediator.Send(new GetProjectByIdQuery(projectId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paginated media for a specific project.
    /// </summary>
    [MapToApiVersion("1.0")]
    [HttpGet("{projectId:guid}/media")]
    [ProducesResponseType(typeof(Paged<ProjectMediaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<ProjectMediaDto>>>> GetProjectMediaAsync(
        [FromRoute] Guid projectId, 
        [FromQuery] int CurrentPage = 1, 
        [FromQuery] int PageSize = 10)
    {
        return (await _mediator.Send(new GetProjectMediaQuery(projectId, CurrentPage, PageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paginated participants for a specific project.
    /// </summary>
    [MapToApiVersion("1.0")]
    [HttpGet("{projectId:guid}/participants")]
    [ProducesResponseType(typeof(Paged<ProjectParticipantDetailsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<ProjectParticipantDetailsDto>>>> GetProjectParticipantsAsync(
        [FromRoute] Guid projectId, 
        [FromQuery] int CurrentPage = 1, 
        [FromQuery] int PageSize = 10)
    {
        return (await _mediator.Send(new GetProjectParticipantsQuery(projectId, CurrentPage, PageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paginated ratings for a specific project.
    /// </summary>
    [MapToApiVersion("1.0")]
    [HttpGet("{projectId:guid}/ratings")]
    [ProducesResponseType(typeof(Paged<ProjectRatingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<ProjectRatingDto>>>> GetProjectRatingsAsync(
        [FromRoute] Guid projectId, 
        [FromQuery] int CurrentPage = 1, 
        [FromQuery] int PageSize = 10)
    {
        return (await _mediator.Send(new GetProjectRatingsQuery(projectId, CurrentPage, PageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves all milestones for a specific project.
    /// </summary>
    [MapToApiVersion("1.0")]
    [HttpGet("{projectId:guid}/milestones")]
    [ProducesResponseType(typeof(List<ProjectMilestoneDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<ProjectMilestoneDto>>>> GetProjectMilestonesAsync([FromRoute] Guid projectId)
    {
        return (await _mediator.Send(new GetProjectMilestonesQuery(projectId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves the source code tree for a specific project.
    /// </summary>
    [MapToApiVersion("1.0")]
    [HttpGet("{projectId:guid}/source-code")]
    [ProducesResponseType(typeof(List<FileNode>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<FileNode>>>> GetProjectSourceCodeAsync([FromRoute] Guid projectId)
    {
        return (await _mediator.Send(new GetProjectSourceCodeQuery(projectId))).ToActionResult(this);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Guid>>> CreateProjectAsync([FromBody] CreateProjectCommand command)
    {
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Changes the status of a project.
    /// </summary>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{projectId:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> ChangeProjectStatusAsync(
        [FromRoute] Guid projectId,
        [FromBody] ChangeProjectStatusCommand command)
    {
        if (projectId != command.ProjectId) return BadRequest(Result.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates the basic info of a project.
    /// </summary>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{projectId:guid}/basic-info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateProjectBasicInfoAsync(
        [FromRoute] Guid projectId,
        [FromBody] UpdateProjectBasicInfoCommand command)
    {
        if (projectId != command.ProjectId) return BadRequest(Result.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates the detailed info of a project.
    /// </summary>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Guid>>> UpdateProjectAsync(
        [FromRoute] Guid projectId,
        [FromBody] UpdateProjectCommand command)
    {
        if (projectId != command.ProjectId) return BadRequest(Result<Guid>.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates the README content of a project.
    /// </summary>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{projectId:guid}/readme")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateProjectReadmeAsync(
        [FromRoute] Guid projectId,
        [FromBody] UpdateProjectReadmeCommand command)
    {
        if (projectId != command.ProjectId) return BadRequest(Result.Failure(SNS.Shared.StatusCodes.OperationStatusCode.InvalidInput));
        return (await _mediator.Send(command)).ToActionResult(this);
    }
}

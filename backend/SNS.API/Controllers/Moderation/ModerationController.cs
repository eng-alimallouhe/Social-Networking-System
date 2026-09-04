using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Attributes;
using SNS.API.Contracts.Moderation.Reports;
using SNS.API.Extensions;
using SNS.Application.Moderation.Commands.ReportComment;
using SNS.Application.Moderation.Commands.ReportCompany;
using SNS.Application.Moderation.Commands.ReportJob;
using SNS.Application.Moderation.Commands.ReportPost;
using SNS.Application.Moderation.Commands.ReportProject;
using SNS.Application.Moderation.Commands.ReportRating;
using SNS.Application.Moderation.Commands.ReportUserProfile;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Moderation.Enums;
using SNS.Infrastructure.Identity.Shared.Authorization;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Moderation;

/// <summary>
/// Handles content moderation and reporting operations.
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ModerationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModerationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Reports a post for a violation.
    /// </summary>
    /// <param name="postId">The unique identifier of the post.</param>
    /// <param name="request">The report details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Post reported successfully.</response>
    /// <response code="400">Invalid report data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user lacks permission.</response>
    /// <response code="404">The post was not found.</response>
    /// <response code="409">The user has already reported this target.</response>
    [HttpPost("posts/{postId:guid}/report")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [RequireSession]
    [HasPermission(Permissions.Moderation.ReportsCreate)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<Guid>>> ReportPostAsync(
        [FromRoute] Guid postId, 
        [FromBody] ReportPostRequest request,
        CancellationToken cancellationToken = default)
    {
        return (await _mediator.Send(new ReportPostCommand(postId, request.ViolationReason, request.AdditionalDetails), cancellationToken)).ToActionResult(this);
    }

    /// <summary>
    /// Reports a comment for a violation.
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment.</param>
    /// <param name="request">The report details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Comment reported successfully.</response>
    /// <response code="400">Invalid report data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user lacks permission.</response>
    /// <response code="404">The comment was not found.</response>
    /// <response code="409">The user has already reported this target.</response>
    [HttpPost("comments/{commentId:guid}/report")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [RequireSession]
    [HasPermission(Permissions.Moderation.ReportsCreate)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<Guid>>> ReportCommentAsync(
        [FromRoute] Guid commentId, 
        [FromBody] ReportCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        return (await _mediator.Send(new ReportCommentCommand(commentId, request.ViolationReason, request.AdditionalDetails), cancellationToken)).ToActionResult(this);
    }

    /// <summary>
    /// Reports a user profile for a violation.
    /// </summary>
    /// <param name="userProfileId">The unique identifier of the profile or user.</param>
    /// <param name="request">The report details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">User profile reported successfully.</response>
    /// <response code="400">Invalid report data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user lacks permission.</response>
    /// <response code="404">The user profile was not found.</response>
    /// <response code="409">The user has already reported this target.</response>
    [HttpPost("user-profiles/{userProfileId:guid}/report")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [RequireSession]
    [HasPermission(Permissions.Moderation.ReportsCreate)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<Guid>>> ReportUserProfileAsync(
        [FromRoute] Guid userProfileId, 
        [FromBody] ReportUserProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        return (await _mediator.Send(new ReportUserProfileCommand(userProfileId, request.ViolationReason, request.AdditionalDetails), cancellationToken)).ToActionResult(this);
    }

    /// <summary>
    /// Reports a rating for a violation.
    /// </summary>
    /// <param name="ratingId">The unique identifier of the rating.</param>
    /// <param name="request">The report details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Rating reported successfully.</response>
    /// <response code="400">Invalid report data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user lacks permission.</response>
    /// <response code="404">The rating was not found.</response>
    /// <response code="409">The user has already reported this target.</response>
    [HttpPost("ratings/{ratingId:guid}/report")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [RequireSession]
    [HasPermission(Permissions.Moderation.ReportsCreate)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<Guid>>> ReportRatingAsync(
        [FromRoute] Guid ratingId, 
        [FromBody] ReportRatingRequest request,
        CancellationToken cancellationToken = default)
    {
        return (await _mediator.Send(new ReportRatingCommand(ratingId, request.ViolationReason, request.AdditionalDetails), cancellationToken)).ToActionResult(this);
    }

    /// <summary>
    /// Reports a project for a violation.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project.</param>
    /// <param name="request">The report details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Project reported successfully.</response>
    /// <response code="400">Invalid report data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user lacks permission.</response>
    /// <response code="404">The project was not found.</response>
    /// <response code="409">The user has already reported this target.</response>
    [HttpPost("projects/{projectId:guid}/report")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [RequireSession]
    [HasPermission(Permissions.Moderation.ReportsCreate)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<Guid>>> ReportProjectAsync(
        [FromRoute] Guid projectId, 
        [FromBody] ReportProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        return (await _mediator.Send(new ReportProjectCommand(projectId, request.ViolationReason, request.AdditionalDetails), cancellationToken)).ToActionResult(this);
    }

    /// <summary>
    /// Reports a company for a violation.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="request">The report details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Company reported successfully.</response>
    /// <response code="400">Invalid report data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user lacks permission.</response>
    /// <response code="404">The company was not found.</response>
    /// <response code="409">The user has already reported this target.</response>
    [HttpPost("companies/{companyId:guid}/report")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [RequireSession]
    [HasPermission(Permissions.Moderation.ReportsCreate)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<Guid>>> ReportCompanyAsync(
        [FromRoute] Guid companyId, 
        [FromBody] ReportCompanyRequest request,
        CancellationToken cancellationToken = default)
    {
        return (await _mediator.Send(new ReportCompanyCommand(companyId, request.ViolationReason, request.AdditionalDetails), cancellationToken)).ToActionResult(this);
    }

    /// <summary>
    /// Reports a job for a violation.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="request">The report details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Job reported successfully.</response>
    /// <response code="400">Invalid report data.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user lacks permission.</response>
    /// <response code="404">The job was not found.</response>
    /// <response code="409">The user has already reported this target.</response>
    [HttpPost("jobs/{jobId:guid}/report")]
    [MapToApiVersion("1.0")]
    [Authorize]
    [RequireSession]
    [HasPermission(Permissions.Moderation.ReportsCreate)]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<Guid>>> ReportJobAsync(
        [FromRoute] Guid jobId, 
        [FromBody] ReportJobRequest request,
        CancellationToken cancellationToken = default)
    {
        return (await _mediator.Send(new ReportJobCommand(jobId, request.ViolationReason, request.AdditionalDetails), cancellationToken)).ToActionResult(this);
    }
}

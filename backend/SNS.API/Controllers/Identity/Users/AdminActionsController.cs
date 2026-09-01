using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.Users.AdminAcions.Commands.ChangeUserRole;
using SNS.Application.Identity.Users.AdminAcions.Commands.PermanentlyBanUser;
using SNS.Application.Identity.Users.AdminAcions.Commands.UnbanUser;
using SNS.Application.Identity.Users.AdminAcions.Queries.GetUserActivityAnalytics;
using SNS.Application.Identity.Users.AdminAcions.Queries.GetUserDetails;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Identity.Users.Entities;
using SNS.Infrastructure.Identity.Shared.Authorization;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.Users;

/// <summary>
/// Handles administrative management actions for user accounts, roles, and administrative analytics.
/// </summary>
[Route("api/v{version:apiVersion}/identity/users/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class AdminActionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminActionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Changes the administrative role assigned to a user account.
    /// </summary>
    /// <remarks>
    /// Requires elevated administrator privileges. Updates user permissions and access rights instantly.
    /// </remarks>
    /// <param name="request">The request payload containing target user ID and new role type.</param>
    /// <response code="200">The user role was successfully updated.</response>
    /// <response code="400">The role update request parameters were invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller does not have administrator authorization.</response>
    /// <response code="404">The target user account was not found.</response>
    [HttpPost("change-user-role")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> ChangeUserRoleAsync([FromBody] ChangeUserRoleCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Permanently bans a user account from accessing the platform.
    /// </summary>
    /// <remarks>
    /// Terminates all active user sessions and revokes access rights permanently.
    /// </remarks>
    /// <param name="request">The ban request payload containing target user ID and reason.</param>
    /// <response code="200">The user was successfully banned.</response>
    /// <response code="400">The ban request details are invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller lacks administrator authorization.</response>
    /// <response code="404">The target user account was not found.</response>
    [HttpPost("permanently-ban-user")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> PermanentlyBanUserAsync([FromBody] PermanentlyBanUserCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Lifts a permanent ban on a user account, restoring account access.
    /// </summary>
    /// <remarks>
    /// Restores user status to active and allows account authentication.
    /// </remarks>
    /// <param name="request">The unban request payload specifying the target user ID.</param>
    /// <response code="200">The user ban was successfully lifted.</response>
    /// <response code="400">The request is invalid.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller lacks administrator authorization.</response>
    /// <response code="404">The target user account was not found.</response>
    [HttpPost("unban-user")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UnbanUserAsync([FromBody] UnbanUserCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves comprehensive activity analytics for a user profile.
    /// </summary>
    /// <remarks>
    /// Returns stats, timeline graph, interaction distribution percentages, and recent activity logs.
    /// </remarks>
    /// <param name="request">The query parameters specifying the target user ID.</param>
    /// <response code="200">Returns the user activity analytics data <see cref="UserActivityAnalyticsResult"/>.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller lacks administrator authorization.</response>
    /// <response code="404">The target user account was not found.</response>
    [HttpGet("user-activity-analytics")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(UserActivityAnalyticsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<UserActivityAnalyticsResult>>> GetUserActivityAnalyticsAsync([FromBody] GetUserActivityAnalyticsQuery request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves detailed user account and profile information for administrative auditing.
    /// </summary>
    /// <remarks>
    /// Includes account status, timestamps, security settings, metrics, and active/historical sessions.
    /// </remarks>
    /// <param name="request">The query parameters identifying the target user ID.</param>
    /// <response code="200">Returns the user details DTO <see cref="UserDetailsDto"/>.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller lacks administrator authorization.</response>
    /// <response code="404">The target user account was not found.</response>
    [HttpGet("user-details")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [HasPermission(Permissions.Users.View)]
    [ProducesResponseType(typeof(UserDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<UserDetailsDto>>> GetUserDetailsAsync([FromBody] GetUserDetailsQuery request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}


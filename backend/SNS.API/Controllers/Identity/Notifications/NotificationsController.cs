using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.Notifications.Commands.MarkAllNotificationsAsRead;
using SNS.Application.Identity.Notifications.Commands.MarkSingleNotificationAsRead;
using SNS.Application.Identity.Notifications.Commands.UpdateNotificationPreferences;
using SNS.Application.Identity.Notifications.Queries.GetNotifications;
using SNS.Application.Identity.Notifications.Queries.GetUserNotificationPreferences;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.Notifications;

/// <summary>
/// Manages user notification retrieval, read state updates, and notification preference settings.
/// </summary>
[Route("api/v{version:apiVersion}/identity/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves user notifications list for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Supports pagination and filtering by unread status.
    /// </remarks>
    /// <param name="request">The query options for retrieving user notifications.</param>
    /// <response code="200">Returns list of notifications <see cref="NotificationDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpGet("user-notification")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<NotificationDto>>> GetUserNotificationAsync([FromBody] GetNotificationsQuery request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves current notification preference channel settings for the authenticated user.
    /// </summary>
    /// <response code="200">Returns notification preferences <see cref="UserNotificationPreferencesDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpGet("user-notification-prefrences")]
    [ProducesResponseType(typeof(UserNotificationPreferencesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<UserNotificationPreferencesDto>>> GetUserNotificationPrefrencesAsync()
    {
        return (await _mediator.Send(new GetUserNotificationPreferencesQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Marks all notifications for the authenticated user as read.
    /// </summary>
    /// <response code="200">All notifications were marked as read successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("mark-all-notification-as-read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> MarkAllNotificationAsRead() 
    {
        return (await _mediator.Send(new MarkAllNotificationsAsReadCommand())).ToActionResult(this);
    }

    /// <summary>
    /// Marks a specific notification as read by ID.
    /// </summary>
    /// <param name="request">The command containing the notification ID to mark as read.</param>
    /// <response code="200">The notification was marked as read successfully.</response>
    /// <response code="400">The notification ID is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The target notification was not found.</response>
    [Authorize]
    [HttpPost("mark-single-notification-as-read")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> MarkAllNotificationAsReadAsync([FromBody] MarkSingleNotificationAsReadCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Updates notification delivery preferences and category notification settings.
    /// </summary>
    /// <param name="request">The updated preferences configuration payload.</param>
    /// <response code="200">Notification preferences were updated successfully.</response>
    /// <response code="400">The preference update payload is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPut("update-notification-prefrences")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> UpdateNotificationPrefrencesAsync([FromBody] UpdateNotificationPreferencesCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}


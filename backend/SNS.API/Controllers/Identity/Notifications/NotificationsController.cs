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

[Route("api/v{version:apiVersion}/identity/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("user-notification")]
    public async Task<ActionResult<Result<NotificationDto>>> GetUserNotificationAsync([FromBody] GetNotificationsQuery request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [Authorize]
    [HttpGet("user-notification-prefrences")]
    public async Task<ActionResult<Result<UserNotificationPreferencesDto>>> GetUserNotificationPrefrencesAsync()
    {
        return (await _mediator.Send(new GetUserNotificationPreferencesQuery())).ToActionResult(this);
    }

    [Authorize]
    [HttpPost("mark-all-notification-as-read")]
    public async Task<ActionResult<Result>> MarkAllNotificationAsRead() 
    {
        return (await _mediator.Send(new MarkAllNotificationsAsReadCommand())).ToActionResult(this);
    }

    [Authorize]
    [HttpPost("mark-single-notification-as-read")]
    public async Task<ActionResult<Result>> MarkAllNotificationAsReadAsync([FromBody] MarkSingleNotificationAsReadCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [Authorize]
    [HttpPut("update-notification-prefrences")]
    public async Task<ActionResult<Result>> UpdateNotificationPrefrencesAsync([FromBody] UpdateNotificationPreferencesCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

}

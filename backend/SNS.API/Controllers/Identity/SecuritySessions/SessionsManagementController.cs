using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.API.Helpers;
using SNS.Application.Identity.SecuritySessions.Login.Commands.RefreshTokens;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.ForceRevokeUserSessions;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.Logout;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.LogoutFromOtherDevices;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.LogoutFromSession;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetSessionDetails;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserActiveSessionsAndDevices;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserSessions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySessions;

/// <summary>
/// Handles user security sessions management, device tracking, session revocation, and logout operations.
/// </summary>
[Route("api/v{version:apiVersion}/identity/security-sessions/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class SessionsManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public SessionsManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Forcefully revokes all security sessions for a target user (Admin privilege required).
    /// </summary>
    /// <remarks>
    /// Administrative endpoint. Invalidates all active refresh tokens for the target user instantly.
    /// </remarks>
    /// <param name="request">The revocation command payload containing target user ID.</param>
    /// <response code="200">User sessions were successfully revoked.</response>
    /// <response code="401">The caller is not authenticated.</response>
    /// <response code="403">The caller does not have administrator authorization.</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("force-revoke-user-sessions")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result>> ForceRevokeUserSessionsAsync([FromBody] ForceRevokeUserSessionsCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Logs out the current user session and clears authentication cookies.
    /// </summary>
    /// <response code="200">Logged out successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> LogoutAsync()
    {
        return (await _mediator.Send(new LogoutCommand())).ToActionResult(this);
    }

    /// <summary>
    /// Revokes a specific security session by session ID.
    /// </summary>
    /// <param name="request">The command containing the target session ID to invalidate.</param>
    /// <response code="200">The session was revoked successfully.</response>
    /// <response code="400">The session ID is invalid or not owned by user.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("logout-from-session")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> LogoutFromSessionAsync([FromBody] LogOutFromSessionCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Logs out the user from all other active device sessions except the current session.
    /// </summary>
    /// <response code="200">All other device sessions were terminated successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("logout-from-other-devices")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> LogoutFromOtherDevicesAsync()
    {
        return (await _mediator.Send(new LogoutFromOtherDevicesCommand())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves detailed security information for a specific session ID.
    /// </summary>
    /// <param name="sessionId">The unique identifier of the target security session.</param>
    /// <response code="200">Returns session security details <see cref="SessionDetaildDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The session ID was not found.</response>
    [Authorize]
    [HttpGet("sessions-details/{sessionId:guid}")]
    [ProducesResponseType(typeof(SessionDetaildDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<SessionDetaildDto>>> GetSessionDeatilsAsync([FromRoute] Guid sessionId)
    {
        return (await _mediator.Send(new GetSessionDetailsQuery(sessionId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves active sessions and registered devices for the authenticated user.
    /// </summary>
    /// <response code="200">Returns active sessions and registered devices <see cref="UserActiveSessionsAndDevicesResult"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpGet("user-active-sessions-and-devices")]
    [ProducesResponseType(typeof(UserActiveSessionsAndDevicesResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<UserActiveSessionsAndDevicesResult>>> GetUserActiveSessionsAndDevicesAsync()
    {
        return (await _mediator.Send(new GetUserActiveSessionsAndDevicesQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves historical user session summaries for a target user ID.
    /// </summary>
    /// <param name="request">The query containing target user ID and pagination parameters.</param>
    /// <response code="200">Returns paged list of session summaries <see cref="SessionSummaryDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpGet("user-sessions/{targetUserId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Paged<SessionSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Paged<SessionSummaryDto>>>> GetUserSessionAsync([FromQuery] GetUserSessionsQuery request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }


    /// <summary>
    /// Re-generate refresh tokens and access tokens for user
    /// the Bearer Token doesn't necessarily have to be inexhaustible, but it must be included.
    /// </summary>
    /// <remarks>
    /// Upon successful authentication, issues access token and sets HTTP-only refresh token cookie.
    /// </remarks>
    /// <response code="200">Returns an object aontains the new access token <see cref="AuthTokenDto"/>.</response>
    /// <response code="401">The token was not included, or the session was expired or revoked.</response>
    [HttpGet("refresh-tokens")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthTokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Paged<AuthTokenDto>>>> RefreshTokensAsync()
    {
        if (!Request.Cookies.TryGetValue(
        CookieFactory.RefreshTokenCookieName,
        out var refreshToken))
        {
            return Unauthorized();
        }

        var result = await _mediator.Send(new RefreshTokensCommand(refreshToken));
        
        if (result.IsSuccess)
        {
            Response.Cookies.Append(
                CookieFactory.RefreshTokenCookieName,
                result.Value?.RefreshToken ?? string.Empty,
                CookieFactory.CreateRefreshTokenCookie(true));
        }

        return (result).ToActionResult(this);
    }
}
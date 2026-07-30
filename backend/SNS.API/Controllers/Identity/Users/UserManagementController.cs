using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserAccount;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Users.UsersManagement.Commands.BeginUserDeactivation;
using SNS.Application.Identity.Users.UsersManagement.Commands.CancelUserDeactivationRequest;
using SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserName;
using SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserPreferredLanguage;
using SNS.Application.Identity.Users.UsersManagement.Commands.CompleteUserDeactivation;
using SNS.Application.Identity.Users.UsersManagement.Queries.checkUsernameAvailabilty;
using SNS.Application.Identity.Users.UsersManagement.Queries.GetUserInformation;
using SNS.Application.Identity.Users.UsersManagement.Queries.GetUserSecurityDetails;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.Users;

/// <summary>
/// Handles user profile settings, username updates, language preferences, account deactivation, and username availability checks.
/// </summary>
[Route("api/v{version:apiVersion}/identity/users/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class UserManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Initiates user account deactivation and dispatches confirmation verification token.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Sends a deactivation confirmation token to user's registered communication channel.
    /// </remarks>
    /// <param name="request">The deactivation command payload.</param>
    /// <response code="200">Returns deactivation initiation token <see cref="BeginUserDeactivationResponse"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("begin-user-deactivation")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(BeginUserDeactivationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<BeginUserDeactivationResponse>>> BeginUserDeactivationAsync([FromBody] BeginUserDeactivationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Cancels a pending user account deactivation request.
    /// </summary>
    /// <param name="request">The cancellation command parameters.</param>
    /// <response code="200">Deactivation cancelled, returns authentication tokens <see cref="AuthTokensDto"/>.</response>
    /// <response code="400">No active deactivation request found or token is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("cancel-user-deactivation")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthTokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthTokensDto>> CancelUserDeactivationAsync([FromBody] CancelUserDeactivationRequestCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Completes user account deactivation using the confirmation token.
    /// </summary>
    /// <param name="request">The completion payload containing deactivation token.</param>
    /// <response code="200">User account deactivated successfully.</response>
    /// <response code="400">The token is invalid or expired.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("complete-user-deactivation")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> CompleteUserDeactivationAsync([FromBody] CompleteUserDeactivationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Updates the username for the authenticated user account.
    /// </summary>
    /// <param name="request">The command containing the new username.</param>
    /// <response code="200">Username updated successfully.</response>
    /// <response code="400">The requested username is invalid or unavailable.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="409">The requested username is already taken by another user.</response>
    [Authorize]
    [HttpPut("update-username")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateUserNameAsync([FromBody] ChangeUserNameCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Updates the preferred application interface language setting.
    /// </summary>
    /// <param name="request">The command payload specifying the new preferred language.</param>
    /// <response code="200">Preferred language updated successfully.</response>
    /// <response code="400">The language setting parameter is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("update-user-preferred-language")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> UpdateUserPreferredLangaugeAsync([FromBody] ChangeUserPreferredLanguageCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves general user account details and settings for the authenticated user.
    /// </summary>
    /// <response code="200">Returns general user account info <see cref="UserInformationResult"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpGet("user-information")]
    [ProducesResponseType(typeof(UserInformationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<UserInformationResult?>>> GetUserInformationAsync()
    {
        return (await _mediator.Send(new GetUserInformationQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves multi-factor security status and recovery option details for the authenticated user.
    /// </summary>
    /// <response code="200">Returns security status details <see cref="UserSecurityDetailsResult"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpGet("user-security-details")]
    [ProducesResponseType(typeof(UserSecurityDetailsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<UserSecurityDetailsResult?>>> GetUserSecurityDetailsAsync()
    {
        return (await _mediator.Send(new GetUserSecurityDetailsQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves the active user account model for session state sync.
    /// </summary>
    /// <response code="200">Returns current user account model <see cref="UserAccount"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [MapToApiVersion("1.0")]
    [Authorize]
    [HttpGet("user-account")]
    [ProducesResponseType(typeof(UserAccount), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<UserAccount>>> GetUserAccountAsync()
    {
        return (await _mediator.Send(new GetUserAccountQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Checks whether a proposed username is available for registration or change.
    /// </summary>
    /// <param name="username">The proposed username to query.</param>
    /// <response code="200">Returns true if username is available; false if taken.</response>
    /// <response code="400">The username string format is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [MapToApiVersion("1.0")]
    [Authorize]
    [HttpGet("username-available")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<bool>>> CheckUsernameAvailabilityAsync([FromQuery] string username)
    {
        return (await _mediator.Send(new CheckUsernameAvailabiltyQuery(username))).ToActionResult(this);
    }
}


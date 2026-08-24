using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySettings.Recovery.Commands.GenerateRecoveryCodes;
using SNS.Application.Identity.SecuritySettings.Recovery.Commands.RecoverAccountBySecurityCode;
using SNS.Application.Identity.SecuritySettings.Recovery.Commands.RevokeRecoveryCodes;
using SNS.Application.Identity.SecuritySettings.Recovery.Queries.GetUserRecoveryCodes;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySettings;

/// <summary>
/// Handles account recovery code generation, revocation, and account recovery authentication workflows.
/// </summary>
[Route("api/v{version:apiVersion}/identity/security-settings/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class RecoveryController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecoveryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Generates a set of single-use emergency account recovery codes for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Hashes and persists new recovery codes, invalidating previous unused codes.
    /// </remarks>
    /// <response code="200">Returns generated plain-text recovery codes list.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpGet("generate-recovery-codes")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> GenerateRecoveryCodesAsync()
    {
        return (await _mediator.Send(new GenerateRecoveryCodesCommand())).ToActionResult(this);
    }

    /// <summary>
    /// Revokes all existing emergency recovery codes for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Invalidates all active recovery codes.
    /// </remarks>
    /// <response code="200">Emergency recovery codes were revoked successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("revoke-recovery-codes")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> RevokeRecoveyrCodesAsync()
    {
        return (await _mediator.Send(new RevokeRecoveryCodesCommand())).ToActionResult(this);
    }

    /// <summary>
    /// Recovers account access using a single-use emergency recovery code.
    /// </summary>
    /// <remarks>
    /// Validates and consumes the recovery code, then issues new authentication tokens.
    /// </remarks>
    /// <param name="request">The account recovery payload containing user identifier and recovery code.</param>
    /// <response code="200">Account recovered successfully, returns new authentication tokens <see cref="AuthTokensDto"/>.</response>
    /// <response code="400">The emergency recovery code is invalid or has already been used.</response>
    /// <response code="404">No matching user account was found.</response>
    [HttpPost("recover-account-by-recovery-code")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthTokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<AuthTokensDto>>> RecoverAccountByRecoveryCodeAsync([FromBody] RecoverAccountBySecurityCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }


    /// <summary>
    /// Get the current user's recovery codes usage history and counts of used and unused codes.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Returns a summary of the user's recovery codes usage, including counts of used and unused codes, and a history of recovery code usage events.
    /// </remarks>
    /// <response code="200">Account recovered successfully, returns new authentication tokens <see cref="AuthTokensDto"/>.</response>
    /// <response code="400">The emergency recovery code is invalid or has already been used.</response>
    /// <response code="404">No matching user account was found.</response>
    [HttpGet]
    [Authorize]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(UserRecoveryCodesDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<UserRecoveryCodesDto?>>> GetUserRecoveryCodesAsync()
    {
        return (await _mediator.Send(new GetUserRecoveryCodesQuery())).ToActionResult(this);
    }


}


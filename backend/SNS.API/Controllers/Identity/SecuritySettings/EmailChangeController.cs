using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.InitialEmailChange;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.ResendEmailChangeVerificationCode;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.VerifyEmailChange;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySettings;

/// <summary>
/// Manages user primary email address updates and verification code workflows.
/// </summary>
[Route("api/v{version:apiVersion}/identity/security-settings/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class EmailChangeController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailChangeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Initiates a primary email address change request and sends a verification code to the new email.
    /// </summary>
    /// <remarks>
    /// Creates a pending email change request and sends an OTP code to the requested new email address.
    /// </remarks>
    /// <param name="request">The request containing the new email address.</param>
    /// <response code="200">Returns email change token and expiration details <see cref="IdentifierChangeResponseDto"/>.</response>
    /// <response code="400">The provided email address is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="409">The new email address is already in use by another account.</response>
    [HttpPut("initiate-email-change")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(IdentifierChangeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<IdentifierChangeResponseDto>>> InitiateEmailChangeAsync([FromBody] InitialEmailChangeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Resends the verification code for an active email change request.
    /// </summary>
    /// <remarks>
    /// Generates a fresh verification code and sends it to the pending new email destination.
    /// </remarks>
    /// <param name="request">The resend command parameters.</param>
    /// <response code="200">Returns updated verification details <see cref="IdentifierChangeResponseDto"/>.</response>
    /// <response code="400">No active pending email change request was found.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPut("resend-email-change-code")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(IdentifierChangeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<IdentifierChangeResponseDto>>> ResendEmailChangeCodeAsync([FromBody] ResendEmailChangeVerificationCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Verifies the OTP verification code and completes the primary email address update.
    /// </summary>
    /// <remarks>
    /// Validates the OTP code, updates the user's primary email address, and issues new authentication tokens.
    /// </remarks>
    /// <param name="request">The verification payload containing the OTP code and token.</param>
    /// <response code="200">Returns fresh authentication tokens <see cref="AuthTokensDto"/>.</response>
    /// <response code="400">The verification code is invalid or expired.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPut("verify-email-change")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthTokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<AuthTokensDto>>> VerifyEmailChangeAsync([FromBody] VerifyEmailChangeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}


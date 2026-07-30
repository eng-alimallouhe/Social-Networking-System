using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.API.Helpers;
using SNS.Application.Identity.SecuritySessions.Login.Commands.CompletePasskeyLogin;
using SNS.Application.Identity.SecuritySessions.Login.Commands.InitiatePasskeyLogin;
using SNS.Application.Identity.SecuritySessions.Login.Commands.LoginWithAuthenticator;
using SNS.Application.Identity.SecuritySessions.Login.Commands.LoginWithPassword;
using SNS.Application.Identity.SecuritySessions.Login.Commands.ResendTwoFactorCode;
using SNS.Application.Identity.SecuritySessions.Login.Commands.ValidateTwoFactorCode;
using SNS.Application.Identity.SecuritySessions.Login.Contracts;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySessions;

/// <summary>
/// Handles user authentication, login credentials validation, 2FA/MFA challenges, and passkey login workflows.
/// </summary>
[Route("api/v{version:apiVersion}/identity/security-sessions/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authenticates a user using username/email and password credentials.
    /// </summary>
    /// <remarks>
    /// Upon successful authentication, issues access token and sets HTTP-only refresh token cookie. Returns MFA challenge if 2FA is enabled.
    /// </remarks>
    /// <param name="request">The login credentials payload.</param>
    /// <response code="200">Returns login outcome details <see cref="LoginResponseDto"/>.</response>
    /// <response code="400">Invalid login parameters or credentials.</response>
    /// <response code="401">Authentication failed due to incorrect password.</response>
    /// <response code="403">The user account is suspended or banned.</response>
    [MapToApiVersion("1.0")]
    [HttpPost("with-password")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<LoginResponseDto>>> LoginWithPasswordAsync([FromBody] LoginWithPasswordCommand request)
    {
        var result = await _mediator.Send(request);

        if (result.IsSuccess)
        {
            Response.Cookies.Append(
                CookieFactory.RefreshTokenCookieName, 
                result.Value?.RefreshToken ?? string.Empty,
                CookieFactory.CreateRefreshTokenCookie(request.RememberMe));
        }

        return (Result<LoginResponseDto>.Success(new LoginResponseDto(
            UserId: result.Value?.UserId,
            DeviceId: result.Value?.DeviceId,
            AccessToken: result.Value?.AccessToken,
            ChallengeToken: result.Value?.ChallengeToken,
            SuspendedUntil: result.Value?.SuspendedUntil,
            SuspensionReason: result.Value?.SuspensionReason,
            RequiresTwoFactor: result.Value?.RequiresTwoFactor ?? false,
            IsMfaRequired: result.Value?.IsMfaRequired ?? false,
            SuspensionReasonCode: result.Value?.SuspensionReasonCode,
            MfaProviderType: result.Value?.MfaProviderType
            ), result.StatusCode)).ToActionResult(this);
    }

    /// <summary>
    /// Completes TOTP multi-factor authentication using an authenticator app verification code.
    /// </summary>
    /// <remarks>
    /// Validates TOTP code against challenge token to issue access and refresh tokens.
    /// </remarks>
    /// <param name="request">The TOTP login command containing challenge token and code.</param>
    /// <response code="200">Returns authentication result <see cref="LoginResponseDto"/>.</response>
    /// <response code="400">The verification code or challenge token is invalid.</response>
    /// <response code="401">MFA challenge validation failed.</response>
    [MapToApiVersion("1.0")]
    [HttpPost("with-authenticator-app")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<LoginResponseDto>>> LoginWithAuthenticatorAppAsync([FromBody] LoginWithAuthenticatorCommand request)
    {
        var result = await _mediator.Send(request);

        if (result.IsSuccess)
        {
            Response.Cookies.Append(
                CookieFactory.RefreshTokenCookieName,
                result.Value?.RefreshToken ?? string.Empty,
                CookieFactory.CreateRefreshTokenCookie(true));
        }

        return (Result<LoginResponseDto>.Success(new LoginResponseDto(
            UserId: result.Value?.UserId,
            DeviceId: result.Value?.DeviceId,
            AccessToken: result.Value?.AccessToken,
            ChallengeToken: result.Value?.ChallengeToken,
            SuspendedUntil: result.Value?.SuspendedUntil,
            SuspensionReason: result.Value?.SuspensionReason,
            RequiresTwoFactor: result.Value?.RequiresTwoFactor ?? false,
            IsMfaRequired: result.Value?.IsMfaRequired ?? false,
            SuspensionReasonCode: result.Value?.SuspensionReasonCode,
            MfaProviderType: result.Value?.MfaProviderType
            ), result.StatusCode)).ToActionResult(this);
    }

    /// <summary>
    /// Initiates WebAuthn passkey authentication challenge.
    /// </summary>
    /// <remarks>
    /// Generates WebAuthn challenge options for client passkey signing.
    /// </remarks>
    /// <param name="request">The passkey login initiation command.</param>
    /// <response code="200">Returns WebAuthn challenge details <see cref="LoginInitialResponseDto"/>.</response>
    /// <response code="400">Passkey initiation parameters are invalid.</response>
    [MapToApiVersion("1.0")]
    [HttpPost("initiate-passkey-login")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(LoginInitialResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<LoginInitialResponseDto>>> InitiatePasskeyLoginAsync([FromBody] InitiatePasskeyLoginCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Completes WebAuthn passkey login by verifying signed assertion response.
    /// </summary>
    /// <remarks>
    /// Validates signed assertion against WebAuthn challenge and completes session establishment.
    /// </remarks>
    /// <param name="request">The WebAuthn passkey assertion response.</param>
    /// <response code="200">Returns login tokens <see cref="LoginResponseDto"/>.</response>
    /// <response code="400">WebAuthn assertion signature validation failed.</response>
    [MapToApiVersion("1.0")]
    [HttpPost("complete-passkey-login")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<LoginResponseDto>>> CompletePasskeyLoginAsync([FromBody] CompletePasskeyLoginCommand request)
    {
        var result = await _mediator.Send(request);

        if (result.IsSuccess)
        {
            Response.Cookies.Append(
                CookieFactory.RefreshTokenCookieName,
                result.Value?.RefreshToken ?? string.Empty,
                CookieFactory.CreateRefreshTokenCookie(true));
        }

        return (Result<LoginResponseDto>.Success(new LoginResponseDto(
            UserId: result.Value?.UserId,
            DeviceId: result.Value?.DeviceId,
            AccessToken: result.Value?.AccessToken,
            ChallengeToken: result.Value?.ChallengeToken,
            SuspendedUntil: result.Value?.SuspendedUntil,
            SuspensionReason: result.Value?.SuspensionReason,
            RequiresTwoFactor: result.Value?.RequiresTwoFactor ?? false,
            IsMfaRequired: result.Value?.IsMfaRequired ?? false,
            SuspensionReasonCode: result.Value?.SuspensionReasonCode,
            MfaProviderType: result.Value?.MfaProviderType
            ), result.StatusCode)).ToActionResult(this);
    }

    /// <summary>
    /// Resends a two-factor authentication (2FA) verification code to user's secondary channel.
    /// </summary>
    /// <remarks>
    /// Generates a new OTP verification code and dispatches it via email or SMS.
    /// </remarks>
    /// <param name="request">The resend 2FA code payload.</param>
    /// <response code="200">The 2FA verification code was resent successfully.</response>
    /// <response code="400">The challenge token is invalid or expired.</response>
    [MapToApiVersion("1.0")]
    [HttpPost("resend-tfa-code")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result>> ResendTfaCodeCodeAsync([FromBody] ResendTwoFactorCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Validates an OTP two-factor verification code to complete authentication.
    /// </summary>
    /// <remarks>
    /// Verifies OTP code against active challenge session and sets refresh cookie.
    /// </remarks>
    /// <param name="request">The 2FA validation command.</param>
    /// <response code="200">Returns access token <see cref="AuthTokenDto"/>.</response>
    /// <response code="400">The 2FA code is invalid or expired.</response>
    [MapToApiVersion("1.0")]
    [HttpPost("validate-tfa-code")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthTokenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<AuthTokenDto>>> ValidateTfaCodeAsync([FromBody] ValidateTwoFactorCommand request)
    {
        var result = await _mediator.Send(request);

        if (result.IsSuccess)
        {
            Response.Cookies.Append(
                CookieFactory.RefreshTokenCookieName,
                result.Value?.RefreshToken ?? string.Empty,
                CookieFactory.CreateRefreshTokenCookie(true));
        }

        return (Result<AuthTokenDto>.Success(new AuthTokenDto(
            Token: result.Value?.Token ?? string.Empty
            ), result.StatusCode)).ToActionResult(this);
    }
}
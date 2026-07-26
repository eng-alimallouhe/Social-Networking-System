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

[Route("api/v{version:apiVersion}/identity/security-sessions/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [MapToApiVersion("1.0")]
    [HttpPost("with-password")]
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

    [MapToApiVersion("1.0")]
    [HttpPost("with-authenticator-app")]
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

    [MapToApiVersion("1.0")]
    [HttpPost("initiate-passkey-login")]
    public async Task<ActionResult<Result<LoginInitialResponseDto>>> InitiatePasskeyLoginAsync([FromBody] InitiatePasskeyLoginCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [MapToApiVersion("1.0")]
    [HttpPost("complete-passkey-login")]
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

    [MapToApiVersion("1.0")]
    [HttpPost("resend-tfa-code")]
    public async Task<ActionResult<Result>> ResendTfaCodeCodeAsync([FromBody] ResendTwoFactorCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [MapToApiVersion("1.0")]
    [HttpPost("validate-tfa-code")]
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
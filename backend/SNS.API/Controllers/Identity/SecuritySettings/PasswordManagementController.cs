using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ChangePassword;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ForgotPassword;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ResendPasswordResetVerificationCode;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ResetPassword;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.VerifyResetPassword;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.DTOs;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySettings;

/// <summary>
/// Handles user password changes, forgot password initiation, verification code validation, and password reset operations.
/// </summary>
[Route("api/identity/password-management/[controller]")]
[ApiController]
[Produces("application/json")]
public class PasswordManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public PasswordManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Changes the password for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Validates old password, updates hash, and re-issues authentication tokens.
    /// </remarks>
    /// <param name="request">The change password payload containing old and new password.</param>
    /// <response code="200">Password changed successfully, returns new tokens <see cref="AuthTokensDto"/>.</response>
    /// <response code="400">The new password does not meet complexity rules or old password is incorrect.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [HttpPost("change-password")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthTokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<AuthTokensDto>>> ChangePasswordAsync([FromBody] ChangePasswordCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Initiates a forgot password workflow by sending a reset verification code to the user's email or phone.
    /// </summary>
    /// <param name="request">The forgot password payload containing user identifier.</param>
    /// <response code="200">Returns password reset challenge details <see cref="PasswordResetResponse"/>.</response>
    /// <response code="400">Invalid user identifier payload.</response>
    /// <response code="404">No matching user account was found.</response>
    [HttpPost("forgot-password")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(PasswordResetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<PasswordResetResponse>>> ForgotPasswordAsync([FromBody] ForgotPasswordCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Resends the password reset OTP verification code.
    /// </summary>
    /// <param name="request">The resend command containing token and user identifier.</param>
    /// <response code="200">Returns updated reset challenge details <see cref="PasswordResetResponse"/>.</response>
    /// <response code="400">Password reset session is invalid or expired.</response>
    [HttpPost("resend-password-reset-verification-code")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(PasswordResetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<PasswordResetResponse>>> ResendPasswordResetVerificationCodeAsync([FromBody] ResendPasswordResetVerificationCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Verifies the OTP code submitted for password reset validation.
    /// </summary>
    /// <param name="request">The verification payload containing OTP code and token.</param>
    /// <response code="200">OTP code verified successfully, returns final reset token <see cref="VerifyResetPasswordResponseDto"/>.</response>
    /// <response code="400">The OTP verification code is invalid or expired.</response>
    [HttpPost("verify-reset-password")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(VerifyResetPasswordResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<VerifyResetPasswordResponseDto>>> VerifyResetPasswordAsync([FromBody] VerifyResetPasswordCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Resets the user's password using a verified password reset token.
    /// </summary>
    /// <param name="request">The reset payload containing reset token and new password.</param>
    /// <response code="200">Password reset complete, returns authentication tokens <see cref="AuthTokensDto"/>.</response>
    /// <response code="400">The reset token is invalid or new password violates password complexity rules.</response>
    [HttpPost("reset-password")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthTokensDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<AuthTokensDto>>> ResetPasswordAsync([FromBody] ResetPasswordCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}


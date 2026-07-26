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

[Route("api/identity/password-management/[controller]")]
[ApiController]
public class PasswordManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public PasswordManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult<Result<AuthTokensDto>>> ChangePasswordAsync([FromBody] ChangePasswordCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult<Result<PasswordResetResponse>>> ForgotPasswordAsync([FromBody] ForgotPasswordCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }


    [HttpPost("resend-password-reset-verification-code")]
    public async Task<ActionResult<Result<PasswordResetResponse>>> ResendPasswordResetVerificationCodeAsync([FromBody] ResendPasswordResetVerificationCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }



    [HttpPost("verify-reset-password")]
    public async Task<ActionResult<Result<VerifyResetPasswordResponseDto>>> VerifyResetPasswordAsync([FromBody] VerifyResetPasswordCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult<Result<AuthTokensDto>>> ResetPasswordAsync([FromBody] ResetPasswordCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}

using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ChangePassword;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ForgotPassword;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.ResetPassword;
using SNS.Application.Identity.SecuritySettings.PasswordManagement.Commands.VerifyResetPassword;

namespace SNS.Api.Controllers.Identity;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/identity/password")]
public class PasswordManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public PasswordManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Password Endpoints

    [HttpPost("change")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("forgot")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("reset")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("verify-reset")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> VerifyReset([FromBody] VerifyResetCodeCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }
}

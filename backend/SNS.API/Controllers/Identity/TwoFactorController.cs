using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySessions.Commands.ResendTwoFactorCode;
using SNS.Application.Identity.SecuritySessions.Commands.ValidateTwoFactorCode;

namespace SNS.Api.Controllers.Identity;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/identity/twofactor")]
public class TwoFactorController : ControllerBase
{
    private readonly IMediator _mediator;

    public TwoFactorController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Two-Factor Endpoints

    [HttpPost("validate")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Validate([FromBody] ValidateTwoFactorCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("resend")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ResendCode([FromBody] ResendTwoFactorCodeCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }
}

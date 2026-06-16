using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySettings.Recovery.Commands.RecoverAccountBySecurityCode;

namespace SNS.Api.Controllers.Identity;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/identity/recovery")]
public class AccountRecoveryController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountRecoveryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Recovery Endpoints

    [HttpPost("recover-by-security-code")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> RecoverAccountBySecurityCode([FromBody] RecoverAccountBySecurityCodeCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }
}

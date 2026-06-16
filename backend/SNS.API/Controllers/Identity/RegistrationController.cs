using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.Users.Registeration.Commands.ActiveAccount;
using SNS.Application.Identity.Users.Registeration.Commands.RegisterUser;
using SNS.Application.Identity.Users.Registeration.Commands.ResendActivationCode;

namespace SNS.Api.Controllers.Identity;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/identity/registration")]
public class RegistrationController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegistrationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Registration Endpoints

    [HttpPost("register")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("activate")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Activate([FromBody] ActivateAccountCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("resend-activation")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> ResendActivation([FromBody] ResendActivationCodeCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }
}

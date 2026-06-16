using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySessions.Commands.LoginWithPassword;
using SNS.Application.Identity.SecuritySessions.Commands.Logout;
using SNS.Application.Identity.SecuritySessions.Commands.RefreshTokens;

namespace SNS.Api.Controllers.Identity;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/identity/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Authentication Endpoints

    [HttpPost("login")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Login([FromBody] LoginWithPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("logout")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LogoutCommand(), cancellationToken);
        return result.ToActionResult(this);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(RefreshTokensCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.ToActionResult(this);
    }
}

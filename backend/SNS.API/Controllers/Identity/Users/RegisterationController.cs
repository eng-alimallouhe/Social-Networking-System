using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.Users.Registeration.Commands.RegisterUser;
using SNS.Application.Identity.Users.Registeration.Commands.ResendVerifyCode;
using SNS.Application.Identity.Users.Registeration.Commands.VerifyUser;
using SNS.Application.Identity.Users.Registeration.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.Users;

[Route("api/[controller]")]
[ApiController]
public class RegisterationController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegisterationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<Result<RegisterResponseDto>>> RegisterAsync([FromBody] RegisterUserCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }


    [HttpPost("resend-verify-code")]
    public async Task<ActionResult<Result<RegisterResponseDto>>> ResendVerifyCodeAsync([FromBody] ResendVerifyCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("verify-user")]
    public async Task<ActionResult<Result<RegisterResponseDto>>> VerifyUserAsync([FromBody] VerifyUserCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}

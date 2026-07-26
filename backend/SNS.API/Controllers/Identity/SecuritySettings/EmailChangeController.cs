using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.InitialEmailChange;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.ResendEmailChangeVerificationCode;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.VerifyEmailChange;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySettings;

[Route("api/v{version:apiVersion}/identity/security-settings/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class EmailChangeController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmailChangeController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("initiate-email-change")]
    public async Task<ActionResult<Result<IdentifierChangeResponseDto>>> InitiateEmailChangeAsync([FromBody] InitialEmailChangeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("resend-email-change-code")]
    public async Task<ActionResult<Result<IdentifierChangeResponseDto>>> ResendEmailChangeCodeAsync([FromBody] ResendEmailChangeVerificationCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("verify-email-change")]
    public async Task<ActionResult<Result<AuthTokensDto>>> VerifyEmailChangeAsync([FromBody] VerifyEmailChangeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}

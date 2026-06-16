using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.InitialEmailChange;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.ResendEmailChangeVerificationCode;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.VerifyEmailChange;

namespace SNS.API.Controllers.Identity
{
    [Route("api/identifierChange/[controller]")]
    [ApiController]
    public class EmailChangeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmailChangeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("initiate-change")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> InitiateChangeAsync([FromBody] InitialEmailChangeCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult(this);
        }


        [HttpPost("resend-verification-code")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> ResendVerificationCaodeAsync(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new ResendEmailChangeVerificationCodeCommand(), cancellationToken);
            return result.ToActionResult(this);
        }

        [HttpPost("verify-change")]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> VerifyChangeAsync([FromBody] VerifyEmailChangeCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return result.ToActionResult(this);
        }
    }
}

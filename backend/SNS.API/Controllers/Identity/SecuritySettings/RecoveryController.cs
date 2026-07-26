using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.ChangeMfaProvider;
using SNS.Application.Identity.SecuritySettings.Recovery.Commands.GenerateRecoveryCodes;
using SNS.Application.Identity.SecuritySettings.Recovery.Commands.RecoverAccountBySecurityCode;
using SNS.Application.Identity.SecuritySettings.Recovery.Commands.RevokeRecoveryCodes;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySettings;

[Route("api/identity/security-settings/[controller]")]
[ApiController]
public class RecoveryController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecoveryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("generate-recovery-codes")]
    public async Task<ActionResult<Result>> GenerateRecoveyrCodesAsync()
    {
        return (await _mediator.Send(new GenerateRecoveryCodesCommand())).ToActionResult(this);
    }

    [Authorize]
    [HttpPost("revoke-recovery-codes")]
    public async Task<ActionResult<Result>> RevokeRecoveyrCodesAsync()
    {
        return (await _mediator.Send(new RevokeRecoveryCodesCommand())).ToActionResult(this);
    }


    [HttpPost("recover-account-by-recovery-code")]
    public async Task<ActionResult<Result<AuthTokensDto>>> RevokeRecoveyrCodesAsync([FromBody] RecoverAccountBySecurityCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}

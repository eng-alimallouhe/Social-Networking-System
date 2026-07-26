using Asp.Versioning;
using Fido2NetLib;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySettings.EmailChange.Commands.ResendRecoveryEmailChangeVerificationCode;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.ChangeDefaultCommunicationMethod;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.ChangeMfaProvider;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.CompleteAuthenticatorRegistration;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.CompletePasskeyRegistration;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.DisableMFA;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.EnableMFA;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.InitialRecoverEmailChange;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.InitiateAuthenticatorRegistration;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.InitiatePasskeyRegistration;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.RemovePasskey;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.VerifyRecoveryEmailChange;
using SNS.Application.Identity.SecuritySettings.MfaManagement.DTOs;
using SNS.Application.Identity.SecuritySettings.Queries.GetUserPasskeys;
using SNS.Application.Identity.SecuritySettings.Queries.GetUserSecuritySettings;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySettings;

[Route("api/v{version:apiVersion}/identity/security-settings/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class MfaManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public MfaManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("change-mfa-provider")]
    public async Task<ActionResult<Result>> ChangeMfaProviderAsync([FromBody] ChangeMfaProviderCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("initiate-authenticator-registration")]
    public async Task<ActionResult<Result<AuthenticatorSetupDto>>> InitiateAuthenticatorRegistrationAsync([FromBody] InitiateAuthenticatorCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [MapToApiVersion("1.0")]
    [HttpPost("complete-authenticator-registration")]
    public async Task<ActionResult<Result>> CompleteAuthenticatorRegistrationAsync([FromBody] CompleteAuthenticatorRegistrationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("initiate-passkey-registration")]
    public async Task<ActionResult<Result<CredentialCreateOptions>>> InitiatePasskeyRegistrationAsync([FromBody] InitiatePasskeyRegistrationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("complete-passkey-registration")]
    public async Task<ActionResult<Result>> CompletePasskeyRegistrationAsync([FromBody] CompletePasskeyRegistrationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("initial-recovery-email-change")]
    public async Task<ActionResult<Result<IdentifierChangeResponseDto>>> InitialRecoveryEmailChangeAsync([FromBody] InitialRecoveryEmailChangeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("resend-recovery-email-change-verification-code")]
    public async Task<ActionResult<Result>> ResendRecoveryEmailChangeVerificationCodeAsync([FromBody] ResendRecoveryEmailChangeVerificationCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("-verify-recovery-email-change")]
    public async Task<ActionResult<Result>> VerifyRecoveryEmailChangeAsync([FromBody] VerifyRecoveryEmailChangeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("change-default-communication-method")]
    public async Task<ActionResult<Result>> ChangeDefaultCommunicationMethodAsync([FromBody] ChangeDefaultCommunicationMethodCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }


    [HttpPost("disable-mfa")]
    public async Task<ActionResult<Result>> DisableMfaAsync()
    {
        return (await _mediator.Send(new DisableMFACommand())).ToActionResult(this);
    }

    [HttpPost("enable-mfa")]
    public async Task<ActionResult<Result>> EnableMfaAsync([FromBody] EnableMFACommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("remove-passkey")]
    public async Task<ActionResult<Result>> RemovePasskeyAsync([FromBody] RemovePasskeyCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpGet("user-passkeys")]
    public async Task<ActionResult<Result<IEnumerable<PasskeyDto>>>> GetUserPasskeysAsync()
    {
        return (await _mediator.Send(new GetUserPasskeysQuery())).ToActionResult(this);
    }

    [HttpGet("user-security-settings")]
    public async Task<ActionResult<Result<IEnumerable<PasskeyDto>>>> GetUserSecuritySettingsAsync()
    {
        return (await _mediator.Send(new GetUserSecuritySettingsQuery())).ToActionResult(this);
    }

}
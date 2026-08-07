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
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.RemoveAuthenticatorApp;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.RemovePasskey;
using SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.VerifyRecoveryEmailChange;
using SNS.Application.Identity.SecuritySettings.MfaManagement.DTOs;
using SNS.Application.Identity.SecuritySettings.Queries.GetUserPasskeys;
using SNS.Application.Identity.SecuritySettings.Queries.GetUserSecuritySettings;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Application.Identity.Users.UsersManagement.Queries.GetUserSecurityDetails;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySettings;

/// <summary>
/// Manages Multi-Factor Authentication (MFA), WebAuthn passkeys, authenticator apps, and security settings.
/// </summary>
[Route("api/v{version:apiVersion}/identity/security-settings/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class MfaManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public MfaManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Changes the active MFA provider type (e.g. Authenticator App, Email, SMS).
    /// </summary>
    /// <param name="request">The request payload specifying the new MFA provider type.</param>
    /// <response code="200">The MFA provider was updated successfully.</response>
    /// <response code="400">The requested MFA provider is invalid or not registered.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("change-mfa-provider")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> ChangeMfaProviderAsync([FromBody] ChangeMfaProviderCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Initiates TOTP authenticator app registration setup.
    /// </summary>
    /// <remarks>
    /// Generates shared secret key and QR code URI for scanning into Google Authenticator or 1Password.
    /// </remarks>
    /// <param name="request">The initiation request payload.</param>
    /// <response code="200">Returns secret key and QR code URI <see cref="AuthenticatorSetupDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("initiate-authenticator-registration")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(AuthenticatorSetupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<AuthenticatorSetupDto>>> InitiateAuthenticatorRegistrationAsync([FromBody] InitiateAuthenticatorCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Completes TOTP authenticator app registration by validating the first TOTP code.
    /// </summary>
    /// <param name="request">The registration completion payload containing the TOTP code.</param>
    /// <response code="200">Authenticator app registered and linked successfully.</response>
    /// <response code="400">The TOTP code is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [MapToApiVersion("1.0")]
    [HttpPost("complete-authenticator-registration")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> CompleteAuthenticatorRegistrationAsync([FromBody] CompleteAuthenticatorRegistrationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Initiates WebAuthn passkey registration options.
    /// </summary>
    /// <remarks>
    /// Generates WebAuthn credential creation challenge options for browser FIDO2 passkey registration.
    /// </remarks>
    /// <param name="request">The passkey registration initiation command.</param>
    /// <response code="200">Returns FIDO2 WebAuthn creation options <see cref="CredentialCreateOptions"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("initiate-passkey-registration")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(CredentialCreateOptions), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<CredentialCreateOptions>>> InitiatePasskeyRegistrationAsync([FromBody] InitiatePasskeyRegistrationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Completes WebAuthn passkey registration by verifying public key attestation.
    /// </summary>
    /// <param name="request">The passkey registration completion payload containing FIDO2 attestation.</param>
    /// <response code="200">Passkey registered successfully.</response>
    /// <response code="400">Passkey attestation verification failed.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("complete-passkey-registration")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> CompletePasskeyRegistrationAsync([FromBody] CompletePasskeyRegistrationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Initiates a recovery email address change request.
    /// </summary>
    /// <param name="request">The command containing the new recovery email address.</param>
    /// <response code="200">Returns verification request details <see cref="IdentifierChangeResponseDto"/>.</response>
    /// <response code="400">The provided recovery email address is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("initial-recovery-email-change")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(IdentifierChangeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<IdentifierChangeResponseDto>>> InitialRecoveryEmailChangeAsync([FromBody] InitialRecoveryEmailChangeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Resends the verification code for a pending recovery email address change.
    /// </summary>
    /// <param name="request">The resend verification code request payload.</param>
    /// <response code="200">The recovery email verification code was resent successfully.</response>
    /// <response code="400">No active recovery email change request was found.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("resend-recovery-email-change-verification-code")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> ResendRecoveryEmailChangeVerificationCodeAsync([FromBody] ResendRecoveryEmailChangeVerificationCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Verifies the OTP code and completes the recovery email address update.
    /// </summary>
    /// <param name="request">The verification payload containing OTP code and token.</param>
    /// <response code="200">Recovery email address updated successfully.</response>
    /// <response code="400">The verification code is invalid or expired.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("-verify-recovery-email-change")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> VerifyRecoveryEmailChangeAsync([FromBody] VerifyRecoveryEmailChangeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Changes the user's default communication channel preference.
    /// </summary>
    /// <param name="request">The request payload specifying the new preferred communication method.</param>
    /// <response code="200">The communication preference was updated successfully.</response>
    /// <response code="400">The specified communication method is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("change-default-communication-method")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> ChangeDefaultCommunicationMethodAsync([FromBody] ChangeDefaultCommunicationMethodCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Disables Multi-Factor Authentication (MFA) on the user's account.
    /// </summary>
    /// <remarks>
    /// Removes requirement for MFA verification during login.
    /// </remarks>
    /// <response code="200">MFA was disabled successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("disable-mfa")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> DisableMfaAsync()
    {
        return (await _mediator.Send(new DisableMFACommand())).ToActionResult(this);
    }

    /// <summary>
    /// Enables Multi-Factor Authentication (MFA) on the user's account.
    /// </summary>
    /// <param name="request">The MFA enable request containing provider configuration.</param>
    /// <response code="200">MFA was enabled successfully.</response>
    /// <response code="400">MFA cannot be enabled (e.g., missing verified MFA provider).</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("enable-mfa")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> EnableMfaAsync([FromBody] EnableMFACommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Removes a registered WebAuthn passkey from the user's account.
    /// </summary>
    /// <param name="request">The command containing the target passkey ID to remove.</param>
    /// <response code="200">The passkey was removed successfully.</response>
    /// <response code="400">The passkey ID was not found or is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpPost("remove-passkey")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result>> RemovePasskeyAsync([FromBody] RemovePasskeyCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves all registered WebAuthn passkeys for the authenticated user.
    /// </summary>
    /// <response code="200">Returns list of registered passkey details <see cref="PasskeyDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpGet("user-passkeys")]
    [ProducesResponseType(typeof(IEnumerable<PasskeyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<IEnumerable<PasskeyDto>>>> GetUserPasskeysAsync()
    {
        return (await _mediator.Send(new GetUserPasskeysQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves security settings configuration status for the authenticated user.
    /// </summary>
    /// <response code="200">Returns current security settings configuration.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpGet("user-security-settings")]
    [ProducesResponseType(typeof(UserSecurityDetailsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<UserSecurityDetailsResult>>> GetUserSecuritySettingsAsync()
    {
        return (await _mediator.Send(new GetUserSecurityDetailsQuery())).ToActionResult(this);
    }


    /// <summary>
    /// Check if the user has an authenticator app linked and remove it.
    /// </summary>
    /// <response code="200">Returns current security settings configuration.</response>
    /// <response code="401">The user or his security settings is not found.</response>
    /// <response code="404">The user is not authenticated.</response>
    [HttpDelete("authenticator-app")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> RemoveAuthenticatorAsync()
    {
        return (await _mediator.Send(new RemoveAuthenticatorAppCommand())).ToActionResult(this);
    }
}
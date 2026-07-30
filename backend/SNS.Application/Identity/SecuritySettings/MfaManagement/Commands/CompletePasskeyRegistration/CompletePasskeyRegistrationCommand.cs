using Fido2NetLib;
using MediatR;
using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.CompletePasskeyRegistration;

/// <summary>
/// Represents a command to complete WebAuthn/FIDO2 passkey registration for the authenticated user.
/// </summary>
/// <param name="AttestationResponse">The raw attestation response returned by the authenticator.</param>
/// <param name="DeviceName">The friendly name of the device registering the passkey.</param>
public sealed record CompletePasskeyRegistrationCommand(
    AuthenticatorAttestationRawResponse AttestationResponse,
    string DeviceName) : ICommand<Unit>;


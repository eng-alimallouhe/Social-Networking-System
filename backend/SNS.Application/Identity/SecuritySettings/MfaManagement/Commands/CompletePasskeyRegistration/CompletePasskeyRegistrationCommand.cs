using Fido2NetLib;
using MediatR;
using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.CompletePasskeyRegistration;

public sealed record CompletePasskeyRegistrationCommand(
    AuthenticatorAttestationRawResponse AttestationResponse,
    string DeviceName) : ICommand<Unit>;

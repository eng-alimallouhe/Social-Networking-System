using Fido2NetLib;
using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.InitiatePasskeyRegistration;

public sealed record InitiatePasskeyRegistrationCommand(string AttestationType = "none") : ICommand<CredentialCreateOptions>;

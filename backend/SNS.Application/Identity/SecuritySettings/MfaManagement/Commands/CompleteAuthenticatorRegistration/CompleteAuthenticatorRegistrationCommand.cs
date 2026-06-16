using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.CompleteAuthenticatorRegistration;

public sealed record CompleteAuthenticatorRegistrationCommand(string Code) : ICommand;

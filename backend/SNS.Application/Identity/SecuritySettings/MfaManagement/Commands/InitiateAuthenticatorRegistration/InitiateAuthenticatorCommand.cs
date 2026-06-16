using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySettings.MfaManagement.DTOs;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.InitiateAuthenticatorRegistration;

public sealed record InitiateAuthenticatorCommand() : ICommand<AuthenticatorSetupDto>;

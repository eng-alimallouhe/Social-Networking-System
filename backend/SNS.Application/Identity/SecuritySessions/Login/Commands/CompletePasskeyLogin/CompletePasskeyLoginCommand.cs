using Fido2NetLib;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Login.Contracts;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.CompletePasskeyLogin;

public sealed record CompletePasskeyLoginCommand(
    Guid UserId,
    AuthenticatorAssertionRawResponse AssertionResponse) : ICommand<LoginInitialResponseDto>;

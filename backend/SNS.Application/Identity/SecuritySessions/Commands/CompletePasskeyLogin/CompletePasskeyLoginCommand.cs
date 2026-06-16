using Fido2NetLib;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.DTOs;

namespace SNS.Application.Identity.SecuritySessions.Commands.CompletePasskeyLogin;

public sealed record CompletePasskeyLoginCommand(
    Guid UserId,
    AuthenticatorAssertionRawResponse AssertionResponse) : ICommand<LoginResponseDto>;

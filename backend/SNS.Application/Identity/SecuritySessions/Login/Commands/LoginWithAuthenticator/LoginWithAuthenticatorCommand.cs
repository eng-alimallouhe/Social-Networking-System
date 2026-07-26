using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Login.Contracts;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.LoginWithAuthenticator;

public record LoginWithAuthenticatorCommand(
    string UserIdentifier,
    string Code
) : ICommand<LoginInitialResponseDto>;

using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Login.Contracts;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.LoginWithPassword;

public sealed record LoginWithPasswordCommand(
    string Identifier, 
    string Password, 
    bool RememberMe = true) : ICommand<LoginInitialResponseDto>;

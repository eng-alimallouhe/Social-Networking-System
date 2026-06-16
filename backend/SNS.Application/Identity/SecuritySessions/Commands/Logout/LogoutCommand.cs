using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySessions.Commands.Logout;

public sealed record LogoutCommand() : ICommand;

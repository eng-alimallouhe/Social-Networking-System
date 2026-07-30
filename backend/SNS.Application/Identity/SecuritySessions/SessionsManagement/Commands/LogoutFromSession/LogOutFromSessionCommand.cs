using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.LogoutFromSession;

public sealed record LogOutFromSessionCommand(Guid SessionId) : ICommand;

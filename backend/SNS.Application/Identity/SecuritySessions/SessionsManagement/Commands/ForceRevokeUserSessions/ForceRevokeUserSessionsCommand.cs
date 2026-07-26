using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.ForceRevokeUserSessions;

public sealed record ForceRevokeUserSessionsCommand(Guid UserId) : ICommand;

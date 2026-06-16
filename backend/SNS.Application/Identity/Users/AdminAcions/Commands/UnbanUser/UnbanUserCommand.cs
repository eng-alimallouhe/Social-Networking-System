using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.AdminAcions.Commands.UnbanUser;

public sealed record UnbanUserCommand(
    Guid TargetUserId) : ICommand;

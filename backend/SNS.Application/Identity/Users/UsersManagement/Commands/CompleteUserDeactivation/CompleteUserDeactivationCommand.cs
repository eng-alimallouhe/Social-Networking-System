using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.CompleteUserDeactivation;

public sealed record CompleteUserDeactivationCommand(
    Guid UserId,
    string Code,
    string Token) : ICommand;

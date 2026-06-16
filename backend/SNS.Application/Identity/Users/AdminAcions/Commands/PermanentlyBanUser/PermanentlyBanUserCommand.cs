using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.AdminAcions.Commands.PermanentlyBanUser;

public sealed record PermanentlyBanUserCommand(
    Guid TargetUserId,
    string Reason) : ICommand;

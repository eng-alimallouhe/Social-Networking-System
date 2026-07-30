using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.AdminAcions.Commands.UnbanUser;

/// <summary>
/// Represents an administrative command to lift a permanent ban from a user account.
/// </summary>
/// <param name="TargetUserId">The unique identifier of the user to be unbanned.</param>
public sealed record UnbanUserCommand(
    Guid TargetUserId) : ICommand;


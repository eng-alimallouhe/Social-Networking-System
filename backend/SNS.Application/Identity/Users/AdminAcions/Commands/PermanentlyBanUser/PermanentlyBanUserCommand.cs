using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.AdminAcions.Commands.PermanentlyBanUser;

/// <summary>
/// Represents an administrative command to permanently ban a user account from the system.
/// </summary>
/// <param name="TargetUserId">The unique identifier of the user to be permanently banned.</param>
/// <param name="Reason">The justification or administrative reason for the permanent ban.</param>
public sealed record PermanentlyBanUserCommand(
    Guid TargetUserId,
    string Reason) : ICommand;


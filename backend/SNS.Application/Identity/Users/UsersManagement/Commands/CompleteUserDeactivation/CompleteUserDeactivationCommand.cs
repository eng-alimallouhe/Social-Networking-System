using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.CompleteUserDeactivation;

/// <summary>
/// Represents a command to complete user account deactivation using a verification code and token.
/// </summary>
/// <param name="UserId">The unique identifier of the user account being deactivated.</param>
/// <param name="Code">The verification code provided for account deactivation.</param>
/// <param name="Token">The deactivation token issued during deactivation initialization.</param>
public sealed record CompleteUserDeactivationCommand(
    Guid UserId,
    string Code,
    string Token) : ICommand;


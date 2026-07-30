using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserName;

/// <summary>
/// Represents a command to update the authenticated user's username.
/// </summary>
/// <param name="NewUserName">The new unique username requested by the user.</param>
public sealed record ChangeUserNameCommand(string NewUserName) : ICommand;


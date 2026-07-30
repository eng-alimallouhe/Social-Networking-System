using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.Login.Contracts;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.LoginWithPassword;

/// <summary>
/// Represents a command to authenticate a user using password credentials.
/// </summary>
/// <param name="Identifier">The user's identifier (email, phone number, or username).</param>
/// <param name="Password">The plain text password submitted for authentication.</param>
/// <param name="RememberMe">Indicates whether to maintain an extended security session duration.</param>
public sealed record LoginWithPasswordCommand(
    string Identifier, 
    string Password, 
    bool RememberMe = true) : ICommand<LoginInitialResponseDto>;


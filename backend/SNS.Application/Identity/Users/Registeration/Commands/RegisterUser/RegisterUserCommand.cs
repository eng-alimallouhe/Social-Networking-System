using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Users.Registeration.DTOs;

namespace SNS.Application.Identity.Users.Registeration.Commands.RegisterUser;

/// <summary>
/// Represents a command to register a new user account with email and password.
/// </summary>
/// <param name="Password">The plain text password for the new user account.</param>
/// <param name="Email">The email address of the user initiating registration.</param>
public sealed record RegisterUserCommand(
    string Password, 
    string Email) : ICommand<RegisterResponseDto>;


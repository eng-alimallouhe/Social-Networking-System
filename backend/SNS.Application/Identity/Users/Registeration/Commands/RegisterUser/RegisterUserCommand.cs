using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Users.Registeration.DTOs;

namespace SNS.Application.Identity.Users.Registeration.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string Password, 
    string Email) : ICommand<RegisterResponseDto>;

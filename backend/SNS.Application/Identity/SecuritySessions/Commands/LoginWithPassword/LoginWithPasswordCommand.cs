using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.DTOs;

namespace SNS.Application.Identity.SecuritySessions.Commands.LoginWithPassword;

public sealed record LoginWithPasswordCommand(string Identifier, string Password) : ICommand<LoginResponseDto>;

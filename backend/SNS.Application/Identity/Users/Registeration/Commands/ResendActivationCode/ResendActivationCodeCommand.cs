using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Users.Registeration.DTOs;

namespace SNS.Application.Identity.Users.Registeration.Commands.ResendActivationCode;

public sealed record ResendActivationCodeCommand(Guid UserId) : ICommand<RegisterResponseDto>;

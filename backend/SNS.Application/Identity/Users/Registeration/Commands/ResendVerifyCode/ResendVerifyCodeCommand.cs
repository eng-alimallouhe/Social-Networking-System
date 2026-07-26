using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Users.Registeration.DTOs;

namespace SNS.Application.Identity.Users.Registeration.Commands.ResendVerifyCode;

public sealed record ResendVerifyCodeCommand(Guid UserId) : ICommand<RegisterResponseDto>;

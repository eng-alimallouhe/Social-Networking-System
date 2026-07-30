using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Users.Registeration.DTOs;

namespace SNS.Application.Identity.Users.Registeration.Commands.ResendVerifyCode;

/// <summary>
/// Represents a command to resend an account verification code to an unverified user.
/// </summary>
/// <param name="UserId">The unique identifier of the user requesting a new verification code.</param>
public sealed record ResendVerifyCodeCommand(Guid UserId) : ICommand<RegisterResponseDto>;


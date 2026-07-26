using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.ValidateTwoFactorCode;

public sealed record ValidateTwoFactorCommand(
    Guid UserId, 
    string Code,
    string Token) : ICommand<AuthTokensDto>;

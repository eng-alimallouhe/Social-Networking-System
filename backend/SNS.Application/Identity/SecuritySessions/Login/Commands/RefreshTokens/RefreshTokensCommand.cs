using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.RefreshTokens;

public sealed record RefreshTokensCommand(
    string refreshToken): ICommand<AuthTokensDto>;

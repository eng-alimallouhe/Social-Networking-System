using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.SecuritySessions.Commands.RefreshTokens;

public sealed record RefreshTokensCommand(
    string RefreshToken) : ICommand<AuthTokensDto>;

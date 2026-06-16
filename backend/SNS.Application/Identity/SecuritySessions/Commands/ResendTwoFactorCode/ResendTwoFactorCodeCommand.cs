using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.SecuritySessions.Commands.ResendTwoFactorCode;

public sealed record ResendTwoFactorCodeCommand(
    Guid UserId,
    CommunicationMethod? ResendMethod = null) : ICommand;

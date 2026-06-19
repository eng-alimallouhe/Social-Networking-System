using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;

namespace SNS.Application.Identity.SecuritySettings.EmailChange.Commands.ResendRecoveryEmailChangeVerificationCode;

public sealed record ResendRecoveryEmailChangeVerificationCodeCommand(
    string Token) : ICommand<IdentifierChangeResponseDto>; 

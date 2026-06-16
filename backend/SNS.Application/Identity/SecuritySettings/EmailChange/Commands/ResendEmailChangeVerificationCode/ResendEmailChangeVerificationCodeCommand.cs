using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;

namespace SNS.Application.Identity.SecuritySettings.EmailChange.Commands.ResendEmailChangeVerificationCode;

public sealed record ResendEmailChangeVerificationCodeCommand() : ICommand<IdentifierChangeResponseDto>; 

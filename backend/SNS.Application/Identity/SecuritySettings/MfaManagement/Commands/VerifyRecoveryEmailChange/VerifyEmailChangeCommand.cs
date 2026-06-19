using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.VerifyRecoveryEmailChange;

public sealed record VerifyRecoveryEmailChangeCommand(
    Guid UserId, 
    string Token, 
    string Code) : ICommand;

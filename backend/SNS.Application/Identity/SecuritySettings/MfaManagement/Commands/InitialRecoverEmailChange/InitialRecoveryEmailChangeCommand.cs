using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.InitialRecoverEmailChange;

public sealed record InitialRecoveryEmailChangeCommand(
    string NewEmail) : ICommand<IdentifierChangeResponseDto>;

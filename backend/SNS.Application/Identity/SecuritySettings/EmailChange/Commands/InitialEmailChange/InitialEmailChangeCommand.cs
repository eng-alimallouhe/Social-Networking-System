using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;

namespace SNS.Application.Identity.SecuritySettings.EmailChange.Commands.InitialEmailChange;

public sealed record InitialEmailChangeCommand(
    string NewEmail) : ICommand<IdentifierChangeResponseDto>;

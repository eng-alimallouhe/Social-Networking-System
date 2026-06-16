using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.Recovery.Commands.GenerateRecoveryCodes;

public sealed record GenerateRecoveryCodesCommand : ICommand<IReadOnlyCollection<string>>;

using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.Recovery.Commands.GenerateRecoveryCodes;

/// <summary>
/// Represents a command to generate a new set of account recovery codes for the authenticated user.
/// </summary>
public sealed record GenerateRecoveryCodesCommand : ICommand<IReadOnlyCollection<string>>;


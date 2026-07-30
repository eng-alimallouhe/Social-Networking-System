using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.Recovery.Commands.RevokeRecoveryCodes;

/// <summary>
/// Represents a command to revoke all active account recovery codes for the authenticated user.
/// </summary>
public sealed record RevokeRecoveryCodesCommand : ICommand;


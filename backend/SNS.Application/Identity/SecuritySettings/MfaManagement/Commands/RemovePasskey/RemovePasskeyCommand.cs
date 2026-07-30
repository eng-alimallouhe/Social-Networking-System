using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.RemovePasskey;

/// <summary>
/// Represents a command to remove a registered passkey from the user's account.
/// </summary>
/// <param name="PasskeyId">The unique identifier of the passkey to be removed.</param>
public record RemovePasskeyCommand(Guid PasskeyId) : ICommand;


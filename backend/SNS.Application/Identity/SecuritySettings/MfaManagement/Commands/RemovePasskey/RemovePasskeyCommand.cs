using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.RemovePasskey;

public record RemovePasskeyCommand(Guid PasskeyId) : ICommand;

using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySessions.Commands.LogoutFromOtherDevices;

public sealed record LogoutFromOtherDevicesCommand(string refreshToken): ICommand;

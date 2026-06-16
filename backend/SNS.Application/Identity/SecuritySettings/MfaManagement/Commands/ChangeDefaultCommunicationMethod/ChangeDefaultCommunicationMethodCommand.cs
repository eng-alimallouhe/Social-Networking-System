using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.ChangeDefaultCommunicationMethod;

public sealed record ChangeDefaultCommunicationMethodCommand(
    CommunicationMethod NewCommunicationMethod): ICommand;

using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Identity.SecuritySettings.Enums;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.ChangeMfaProvider;

public sealed record ChangeMfaProviderCommand(MfaProvider NewProvider) : ICommand;

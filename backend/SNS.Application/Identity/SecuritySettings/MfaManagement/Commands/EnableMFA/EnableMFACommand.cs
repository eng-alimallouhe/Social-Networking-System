using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Shared.Exceptions;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.EnableMFA;

public sealed record EnableMFACommand(MfaProvider MfaProvider): ICommand;

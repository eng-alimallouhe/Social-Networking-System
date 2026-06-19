using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserPreferredLanguage;

public sealed record ChangeUserPreferredLanguageCommand(SupportedLanguage NewLanguage) : ICommand;

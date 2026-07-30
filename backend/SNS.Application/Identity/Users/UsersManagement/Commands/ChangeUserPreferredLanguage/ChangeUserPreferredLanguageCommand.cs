using SNS.Application.Abstractions.Messaging;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserPreferredLanguage;

/// <summary>
/// Represents a command to change the authenticated user's preferred application language.
/// </summary>
/// <param name="NewLanguage">The new preferred language setting for the user.</param>
public sealed record ChangeUserPreferredLanguageCommand(SupportedLanguage NewLanguage) : ICommand;


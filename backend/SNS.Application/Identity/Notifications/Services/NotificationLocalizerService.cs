using SNS.Application.Identity.Notifications.Abstractions;
using SNS.Application.Identity.Notifications.Contracts;
using SNS.Domain.Identity.Notifications.Enums;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Exceptions;
using System.Collections.Concurrent;
using System.Text.Json;

public class NotificationLocalizerService
    : INotificationLocalizerService
{
    private const SupportedLanguage DefaultLanguage = SupportedLanguage.English;

    private readonly ConcurrentDictionary<SupportedLanguage, NotificationTranslation> _cache = new();

    public NotificationContent Localize(
        NotificationType type,
        SupportedLanguage language,
        NotificationArguments arguments)
    {
        var translations = _cache.GetOrAdd(language, LoadTranslations);

        if (!translations.TryGetValue(type.ToString(), out var template))
        {
            throw new ResourceNotFoundException(
                $"Notification translation for '{type}' was not found.");
        }

        return new NotificationContent(
            template.Title,
            string.Format(template.Body, arguments.ActorName));
    }

    private NotificationTranslation LoadTranslations(SupportedLanguage language)
    {
        var path = GetFilePath(language);

        if (!File.Exists(path))
        {
            if (language != DefaultLanguage)
                return LoadTranslations(DefaultLanguage);

            throw new FileNotFoundException(path);
        }

        var json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<NotificationTranslation>(json)!
               ?? throw new InvalidOperationException("Invalid translation file.");
    }

    private static string GetFilePath(SupportedLanguage language)
    {
        return Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            "Translations",
            "Notifications",
            language.ToString(),
            "Notifications.json");
    }
}
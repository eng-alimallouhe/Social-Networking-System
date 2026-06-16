using SNS.Application.Abstractions.Messaging;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Exceptions;
using System.Text;

namespace SNS.Infrastructure.Shared.Services.Messaging;

public class EmailTemplateProvider : IEmailTemplateProvider
{
    private const SupportedLanguage DefaultLanguage = SupportedLanguage.English;

    public async Task<EmailContent> ReadTemplate(
        SupportedLanguage language,
        SendPurpose purpose,
        IReadOnlyList<MessageReplacement> replacements)
    {
        var path = GetFilePath(language, purpose);

        if (!File.Exists(path))
        {
            if (DefaultLanguage != language)
            {
                path = GetFilePath(DefaultLanguage, purpose);
            }

            if (!File.Exists(path))
            {
                throw new ResourceNotFoundException(
                    $"CRITICAL: Failed to load the template for '{purpose}'. Fallback also failed at '{path}'.");
            }
        }

        var json = await File.ReadAllTextAsync(path);

        var emailTemplate = System.Text.Json.JsonSerializer.Deserialize<EmailContent>(json);

        if (emailTemplate == null)
            throw new ResourceNotFoundException($"Failed to deserialize the template at '{path}'.");

        var subject = emailTemplate.Subject;

        StringBuilder body = new StringBuilder(emailTemplate.Body);

        foreach (var replacement in replacements)
        {
            body.Replace("{{" + $"{replacement.Key}" + "}}", replacement.Value);
        }

        return new EmailContent(
            Subject: subject,
            Body: body.ToString());
    }


    private string GetFilePath(SupportedLanguage language, SendPurpose purpose)
    {
        return Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Resources",
            "MessagingTemplates",
            "RecoveryEmail",
            language.ToString(),
            $"{purpose}.json");
    }
}

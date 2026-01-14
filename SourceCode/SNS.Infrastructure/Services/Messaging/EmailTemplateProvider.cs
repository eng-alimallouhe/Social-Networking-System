using SNS.Application.Abstractions.Messaging;
using SNS.Common.Enums;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Common;
using SNS.Domain.Common.Enums;

namespace SNS.Infrastructure.Services.Messaging;

public class EmailTemplateProvider : IEmailTemplateProvider
{
    public Result<string> ReadTemplate(
        SupportedLanguage language,
        SendPurpose purpose,
        IReadOnlyDictionary<string, string> replacements)
    {
        var templatePath = string.Empty;

        try
        {
            var path = Path
                .Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "MessagingTemplates", "Email",
                language.ToString(),
                $"{purpose.ToString()}.html");

            if (!File.Exists(path))
            {
                return Result<string>.Failure(ResourceStatusCode.NotFound);
            }

            foreach (var replacement in replacements)
            {
                path = path.Replace("{{" + $"{replacement.Key}" + "}}", replacement.Value);
            }

            templatePath = File.ReadAllText(path);
        }
        catch
        {
            return Result<string>.Failure(OperationStatusCode.ServerError);
        }

        return Result<string>.Success(templatePath, ResourceStatusCode.Found);
    }
}

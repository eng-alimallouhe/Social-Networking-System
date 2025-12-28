using SNS.Application.Abstractions.Messaging;
using SNS.Common.Enums;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Common;

namespace SNS.Infrastructure.Services.Messaging;

public class TemplateReaderService : ITemplateReaderService
{
    public Result<string?> ReadTemplate(SupportedLanguage language, TemplatePurpose purpose)
    {
        var templatePath = string.Empty;

        try
        {
            var path = Path
                .Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "EmailTemplates",
                language.ToString().ToUpper(), 
                $"{purpose.ToString()}.html");

            if (!File.Exists(path))
            {
                return Result<string?>.Failure(Resources.ResourceNotFound);
            }

            templatePath = File.ReadAllText(path);
        }
        catch
        {
                return Result<string?>.Failure(Resources.ResourceReadError);
        }

        return Result<string?>.Success(templatePath, Resources.ResourceFound);
    }
}

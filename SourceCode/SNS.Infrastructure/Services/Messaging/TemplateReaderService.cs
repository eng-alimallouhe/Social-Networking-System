using SNS.Application.Abstractions.Messaging;
using SNS.Common.Enums;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Common;
using SNS.Domain.Common.Enums;

namespace SNS.Infrastructure.Services.Messaging;

public class TemplateReaderService : ITemplateReaderService
{
    public Result<string?> ReadTemplate(SupportedLanguage language, SendPurpose purpose)
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
                return Result<string?>.Failure(ResourceStatusCode.NotFound);
            }

            templatePath = File.ReadAllText(path);
        }
        catch
        {
                return Result<string?>.Failure(ResourceStatusCode.ReadError);
        }

        return Result<string?>.Success(templatePath, ResourceStatusCode.Found);
    }
}

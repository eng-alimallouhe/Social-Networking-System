using SNS.Common.Enums;
using SNS.Common.Results;

namespace SNS.Application.Abstractions.Messaging;

public interface ITemplateReaderService
{
    Result<string?> ReadTemplate(SupportedLanguage language, TemplatePurpose purpose);
}

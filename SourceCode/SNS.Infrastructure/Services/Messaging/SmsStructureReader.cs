using SNS.Application.Abstractions.Messaging;
using SNS.Common.Enums;
using SNS.Domain.Common.Enums;

namespace SNS.Infrastructure.Services.Messaging;

public class SmsStructureReader : ISmsStructureReader
{
    public string? GetSmsBody(SupportedLanguage language, SendPurpose purpose)
    {
        throw new NotImplementedException();
    }
}

using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.VerificationCodes;

public sealed record CodeSendRequest(
    Guid UserId,
    string UserName,
    string RecipientAddress,
    SendPurpose Purpose,
    CommunicationMethod SendMethod,
    SupportedLanguage SendLanguage,
    string RedirectUrl,
    string Token);

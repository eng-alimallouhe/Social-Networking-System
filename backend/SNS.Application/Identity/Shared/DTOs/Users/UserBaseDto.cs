using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.Users;

public sealed record UserBaseDto(
    Guid Id,
    string UserName,
    CommunicationMethod DefaultCommunicationMethod,
    SupportedLanguage PreferredLanguage,
    string? RecoveryEmail,
    string Email);

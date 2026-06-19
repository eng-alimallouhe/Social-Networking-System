using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.SecuritySettings.Queries.GetUserSecuritySettings;

public sealed record UserSecuritySettingsDto(
    bool IsMfaEnabled,
    bool IsAuthenticatorLinked,
    MfaProvider? MfaProvider,
    string? RecoveryEmail,
    CommunicationMethod DefaultCommunicationMethod,
    int ActiveRecoveryCodesCount);

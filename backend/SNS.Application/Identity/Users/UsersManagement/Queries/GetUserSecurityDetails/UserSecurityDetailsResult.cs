namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserSecurityDetails;

public sealed record UserSecurityDetailsResult(
    bool IsMfaEnabled,
    string MfaProvider,
    bool IsAuthenticatorAppLinked,
    int PasskeysCount,
    DateTime LastPasswordChange,
    int TotalDevicesCount,
    string? RecoveryEmail,
    int UsedRecoveryCodesCount,
    int UnusedRecoveryCodesCount);

namespace SNS.Domain.Security.Enums;

public enum ActionType
{
    // Authentication Actions
    Login,
    Logout,

    // Security Actions
    PasswordChanged,
    EmailChanged,
    PhoneChanged,
    TwoFactorEnabled,
    TwoFactorDisabled,
    SecurityCodeGenerated,
    SupportResetPhoneNumber,

    // Moderation / Enforcement Actions
    Suspended,
    SuspensionLifted,
    Banned,
    BanLifted,

    // Account Lifecycle
    AccountCreated,
    AccountActivated,
    AccountDeactivated,
    AccountDeleted,

    // Administrative Actions
    RoleChanged,
    ManualRecoveryRequested,
    ManualRecoveryReviewed
}

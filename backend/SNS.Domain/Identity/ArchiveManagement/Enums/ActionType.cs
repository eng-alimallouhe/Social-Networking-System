namespace SNS.Domain.Identity.ArchiveManagement.Enums;

public enum ActionType
{
    // Authentication Actions
    Login,
    Logout,

    // Security Actions
    PasswordChanged,
    EmailChanged,
    TwoFactorEnabled,
    TwoFactorDisabled,
    SecurityCodeGenerated,

    // Moderation / Enforcement Actions
    Suspended,
    SuspensionLifted,
    SuspendedDueMaxFailedLoginAttempts,
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

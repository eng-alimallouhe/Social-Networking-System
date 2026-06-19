namespace SNS.Domain.Identity.Shared.Enums;

public enum SendPurpose
{
    UserDeactivated = 0,

    UserVerification = 1,


    LoginTwoFactor = 2,

    PasswordReset = 3,

    EmailChangeVerification = 4,
    
    RecoveryEmailChangeVerification = 4,

    PasswordChangedAlert = 10,

    EmailChangedAlert = 11,

    LoginAlert = 12,

    LoginWithSecurityCodeAlert = 13,

    UserDeleting = 14,
}

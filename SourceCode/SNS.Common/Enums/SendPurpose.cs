namespace SNS.Common.Enums;

public enum SendPurpose
{
    AccountActivation = 1,

    LoginTwoFactor = 2,

    PasswordReset = 3,

    EmailChangeVerification = 4,

    PhoneChangeVerification = 5,

    SupportPhoneChangeRequest = 6,

    PasswordChangedAlert = 10,

    PhoneNumberChangedAlert = 11,

    EmailChangedAlert = 12
}
namespace SNS.Domain.Security;

public enum CodeType {
    AccountActivation,
    LoginTwoFactor,
    PasswordReset,
    ChangeEmail,
    ChangePhoneNumber,
    SupportChangePhoneNumber
}
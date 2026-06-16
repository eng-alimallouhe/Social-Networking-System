namespace SNS.Domain.Identity.SecuritySettings.Enums;

public enum MfaProvider
{
    None = 0,
    RecoveryEmail = 1,
    Email = 2,
    AuthenticatorApp = 3,
    Passkey = 4,
}
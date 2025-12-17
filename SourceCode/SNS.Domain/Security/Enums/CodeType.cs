namespace SNS.Domain.Security;

public enum CodeType { 
    NewAnnouncement,
    NewLogin,
    PasswordChanged,
    TwoFactorEnabled,
    TwoFactorDisabled
}

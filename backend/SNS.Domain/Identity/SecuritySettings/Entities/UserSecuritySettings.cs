using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Shared.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.SecuritySettings.Entities;

public class UserSecuritySettings : Entity, IHardDeletable
{
    public Guid Id { get; set; }

    // Primary key and foreign key to User entity
    public Guid UserId { get; private set; }

    //Unique Field: RecoveryEmail
    public string? RecoveryEmail { get; private set; }
    public bool FailedLoginNotifications { get; private set; }
    public bool LoginAlerts { get; private set; }
    public bool PasswordChangeAlerts { get; private set; }
    public string AuthenticatorSecretKey { get; private set; } = string.Empty;

    [NotMapped]
    public bool IsAuthenticatorLinked => string.IsNullOrEmpty(AuthenticatorSecretKey);

    [NotMapped]
    public bool IsMfaEnabled => MfaProvider != MfaProvider.None;

    public MfaProvider MfaProvider { get; private set; } = MfaProvider.None;


    public CommunicationMethod DefaultCommunicationMethod { get; private set; }


    public ICollection<RecoveryCode> RecoveryCodes { get; set; } = new List<RecoveryCode>();
    public ICollection<Device> Devices { get; set; } = new List<Device>();

    private UserSecuritySettings()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        DefaultCommunicationMethod = CommunicationMethod.Email;
        FailedLoginNotifications = false;
    }

    public static UserSecuritySettings Create(Guid userId, string? recoveryEmail)
    {
        var entity = new UserSecuritySettings();
        entity.UserId = userId;
        entity.RecoveryEmail = recoveryEmail;
        return entity;
    }

    public void SetRecoveryEmail(string email)
    {
        if (RecoveryEmail != null)
            throw new DomainException("The email already founded, try to change it");

        RecoveryEmail = email;
    }

    public void ChangeRecoveryEmail(string email)
    {
        this.RecoveryEmail = email;
    }

    public void ChangeDefaultCommunicationMethod(CommunicationMethod method)
    {
        if (method == CommunicationMethod.RecoveryEmail && string.IsNullOrEmpty(this.RecoveryEmail))
            throw new InvalidOperationException("Cannot set default communication method to RecoveryEmail without a recovery email.");

        this.DefaultCommunicationMethod = method;
    }

    public string InitiateAuthenticatorSetup()
    {
        string newSecret = GenerateBase32Secret(16);

        this.AuthenticatorSecretKey = newSecret;

        return newSecret;
    }

    public void ChangeMfaProvider(MfaProvider provider)
    {
        if (provider == MfaProvider.RecoveryEmail && string.IsNullOrEmpty(this.RecoveryEmail))
        {
            throw new DomainException("Cannot set MFA provider to RecoveryEmail without a recovery email.");
        }

        this.MfaProvider = provider;
    }

    public void DisableMfa()
    {
        if (MfaProvider == MfaProvider.None)
        {
            throw new DomainException("MFA already disabled!");
        }
        this.MfaProvider = MfaProvider.None;
    }

    public void EnableMfa(MfaProvider mfaProvider)
    {
        if (IsMfaEnabled)
        {
            throw new DomainException("Mfa already enabled");
        }

        if (mfaProvider == MfaProvider)
        {
            throw new DomainException("This provider already used");    
        }

        this.MfaProvider = mfaProvider;
    }

    public void EnableAuthenticator()
    {
        if (string.IsNullOrEmpty(AuthenticatorSecretKey))
            throw new InvalidOperationException("Cannot enable authenticator without setup.");

        this.MfaProvider = MfaProvider.AuthenticatorApp;
    }

    // دالة مساعدة لتوليد نص عشوائي متوافق مع Base32
    private string GenerateBase32Secret(int length)
    {
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        
        var random = new Random();
        
        var chars = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            chars[i] = validChars[random.Next(validChars.Length)];
        }
        
        return new string(chars);
    }
}

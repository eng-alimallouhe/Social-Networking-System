using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Identity.Notifications.Entities;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Helpers;
using SNS.Shared.Exceptions;

namespace SNS.Domain.Identity.Users.Entities;

public class User : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Role) To Many(Users)
    public Guid RoleId { get; private set; }


    public string UserName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public int FailedLoginAttempts { get; private set; }
    public SupportedLanguage PreferredLanguage { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; } 
    public DateTime UpdatedAt { get; private set; } 
    public DateTime LastLogIn { get; private set; } 
    public DateTime LastPasswordChange { get; private set; }
    
    public UserStatus Status { get; private set; }
    public DateTime? SuspendedUntil { get; private set; }
    public string? SuspensionReason { get; private set; }
    
    public DateTime? DeactivatedAt { get; private set; }

    // Soft Delete
    public bool IsVerified { get; private set; }
    public bool PurgeAllContentOnHardDelete { get; private set; }


    // Security Code
    public DateTime CodeCreatedAt { get; private set; }


    // Navigation Properties
    public Profile UserProfile { get; set; } = null!;
    public ICollection<UserPasskey> Passkeys { get; private set; } = new List<UserPasskey>();
    public UserSecuritySettings UserSecuritySettings { get; private set; } = null!;
    public Role Role { get; private set; } = null!;
    public ICollection<IdentityArchive> IdentityArchives { get; private set; } = new List<IdentityArchive>();
    public ICollection<PasswordArchive> PasswordArchives { get; private set; } = new List<PasswordArchive>();
    public ICollection<UserArchive> Archives { get; private set; } = new List<UserArchive>();
    public ICollection<UserArchive> ActionPerformed { get; private set; } = new List<UserArchive>();
    public ICollection<Notification> Notifications { get; private set; } = new List<Notification>();
    public ICollection<SecuritySession> Sessions { get; private set; } = new List<SecuritySession>();
    public ICollection<Device> Devices { get; private set; } = new List<Device>();
    public UserNotificationPreferences NotificationPreferences { get; private set; } = null!;

    private User()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        LastLogIn = DateTime.UtcNow;
        PreferredLanguage = SupportedLanguage.English;
        LastPasswordChange = DateTime.UtcNow;
        CodeCreatedAt = DateTime.UtcNow;
        Status = UserStatus.Active;
        DeactivatedAt = null;
        PurgeAllContentOnHardDelete = false;
    }


    public static User Create(Guid roleId, string userName, string email, string passwordHash)
    {
        var entity = new User();
        entity.RoleId = roleId;
        entity.UserName = userName;
        entity.Email = email;
        entity.PasswordHash = passwordHash;
        return entity;
    }
    
    public static User CreateDefaultUser()
    {
        var user = new User();

        user.Id = SystemUsers.GhostUserId;
        user.RoleId = SystemRoles.GhostRoleId;
        user.CreatedAt = new DateTime(1, 1, 1);
        user.UpdatedAt = new DateTime(1, 1, 1);
        user.LastLogIn = new DateTime(1, 1, 1);
        user.LastPasswordChange = new DateTime(1, 1, 1);
        user.CodeCreatedAt = new DateTime(1, 1, 1);
        
        return user;
    }

    public void ChangePassword(string hashedPassword)
    {
        this.PasswordHash = hashedPassword;
        this.LastPasswordChange = DateTime.UtcNow;
    }

    public void ChangeRole(Guid newRoleId)
    {
        this.RoleId = newRoleId;
    }

    public void ChangeUserName(string newUserName) { this.UserName = newUserName; }

    public void ChangeEmail(string email)
    {
        this.Email = email;
    }

    public void ChangeUserPreferredLanguage(SupportedLanguage language) { this.PreferredLanguage = language; }

    public void SetSecuritySettings(UserSecuritySettings settings)
    {
        this.UserSecuritySettings = settings;
    }

    public void Verify()
    {
        this.IsVerified = true;
    }

    public void Suspend(
        DateTime suspendedUntil, 
        string? reason = null)
    {
        if (Status == UserStatus.Suspended)
            throw new DomainException("User already suspended");

        Status = UserStatus.Suspended;
        SuspendedUntil = suspendedUntil;
        SuspensionReason = reason;
    }

    public void Activate()
    {
        if (Status == UserStatus.Active)
            throw new DomainException("User already active");

        Status = UserStatus.Active;
        DeactivatedAt = null;
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Deactivated)
            throw new DomainException("User already deactivated");

        Status = UserStatus.Deactivated;
        DeactivatedAt = DateTime.UtcNow;
    }

    public void PermanentlyBan()
    {
        if (Status == UserStatus.PermanentlyBanned)
        {
            throw new DomainException("User already banned!");
        }
        this.Status = UserStatus.PermanentlyBanned;
    }
    
    public void UnBan()
    {
        if (Status != UserStatus.PermanentlyBanned)
        {
            throw new DomainException("User already unbanned!");
        }
        this.Status = UserStatus.Active;
    }


    public void ResetFailedLoginAttempts()
    {
        this.FailedLoginAttempts = 0;
    }

    public void IncrementFailedLoginAttempts()
    {
        this.FailedLoginAttempts++;
    }



    public void ChangePreferredLanguage(SupportedLanguage language)
    {
        this.PreferredLanguage = language;
    }
}

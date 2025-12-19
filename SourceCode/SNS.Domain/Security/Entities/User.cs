using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Security;

public class User : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; } = Guid.NewGuid();

    // Foreign Key: One(Role) To Many(Users)
    public Guid RoleId { get; set; }


    public required string UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string PasswordHash { get; set; }
    public int FailedLoginAttempts { get; set; }


    // Timestamp
    public DateTime CreatedAt { get; set; } 
    public DateTime UpdatedAt { get; set; } 
    public DateTime LastLogIn { get; set; } 
    public DateTime LastPasswordChange { get; set; }

    
    public bool IsTwoFactorEnabled { get; set; } 
    public bool IsBanned { get; set; }
    public bool IsSuspended { get; set; } 
    public DateTime? SuspendedUntil { get; set; }

    // Soft Delete
    public bool IsActive { get; set; } 

    
    // Security Code
    public required string SecurityCode { get; set; } 
    public DateTime CodeCreatedAt { get; set; }


    // Navigation Properties
    public Role Role { get; set; } = null!;
    public RefreshToken Token { get; set; } = null!;
    public ICollection<VerificationCode> VerificationCodes { get; set; } = new List<VerificationCode>();
    public ICollection<IdentityArchive> IdentityArchives { get; set; } = new List<IdentityArchive>();
    public ICollection<PasswordArchive> PasswordArchives { get; set; } = new List<PasswordArchive>();
    public ICollection<PendingUpdate> PendingUpdates { get; set; } = new List<PendingUpdate>();
    public ICollection<UserArchive> Archives { get; set; } = new List<UserArchive>();
    public ICollection<UserArchive> ActionPerformed { get; set; } = new List<UserArchive>();
    public ICollection<Notification> Notification { get; set; } = new List<Notification>();
    public ICollection<ManualRecoveryRequest> RecoveryRequests { get; set; } = new List<ManualRecoveryRequest>();
    public ICollection<ManualRecoveryRequest> RecoveryReviews { get; set; } = new List<ManualRecoveryRequest>();
    public ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
    public ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();

}

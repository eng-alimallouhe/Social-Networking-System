
using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Security.Entities;

public class UserSession : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; } 


    // Foreign Key: One(User) To Many(Sessions)
    public Guid UserId { get; set; }


    // Timestamp
    public DateTime LoginAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public DateTime? LogoutAt { get; set; }


    public int? DurationMinutes { get; set; }
    public required string IpAddress { get; set; }
    public required string Device { get; set; }
    public required string Browser { get; set; }
    public required string Country { get; set; }
    public bool IsActive { get; set; }


    // Navigation
    public User User { get; set; } = null!;

    public UserSession()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        LoginAt = DateTime.UtcNow;
        LastSeenAt = DateTime.UtcNow;
        IsActive = true;
    }
}

namespace SNS.Domain.Security;

public class UserSession
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
    public bool IsActive { get; set; } = true;


    // Navigation
    public User User { get; set; } = null!;
}

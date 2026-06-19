using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.SecuritySessions.Entities;

public class SecuritySession : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; } 


    // Foreign Key: One(User) To Many(Sessions)
    public Guid UserId { get; private set; }
    public Guid DeviceId { get; set; }

    // Timestamp
    public DateTime LoginAt { get; private set; }
    public DateTime LastSeenAt { get; private set; }
    public DateTime? LogoutAt { get; private set; }


    public string IpAddress { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public double Longitude { get; private set; }
    public double Latitude { get; private set; }
    public bool IsActive { get; private set; }
    public int DurationMinutes { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public string? RevokedReason { get; private set; }

    // Navigation
    public ICollection<RefreshToken> RefreshTokens { get; set; } 
        = new List<RefreshToken>();

    public Device Device { get; set; } = null!;

    private SecuritySession()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        LoginAt = DateTime.UtcNow;
        LastSeenAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static SecuritySession Create(
        Guid userId, 
        Guid deviceId, 
        string ipAddress,
        string country, 
        string city, 
        int durationMinutes)
    {
        var entity = new SecuritySession()
        {
            IpAddress = ipAddress,
            City = city,
            DeviceId = deviceId,
            Country = country
        };
        entity.UserId = userId;
        entity.DurationMinutes = durationMinutes;
        return entity;
    }

    public void UpdateLastSeen()
    {
        this.LastSeenAt = DateTime.UtcNow;
    }

    public void Logout(DateTime at)
    {
        this.LogoutAt = at;
        this.IsActive = false;
    }

    public void SetDurationMinutes(int durationMinutes)
    {
        this.DurationMinutes = durationMinutes;
    }

    public void Revoke(string? reason)
    {
        IsActive = false;
        LogoutAt = DateTime.UtcNow;
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        RevokedReason = reason;
        foreach (var token in RefreshTokens)
        {
            token.Revoke();
        }
    }
}

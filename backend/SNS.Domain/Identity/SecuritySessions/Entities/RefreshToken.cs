using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Identity.SecuritySessions.Entities;

public class RefreshToken : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }


    // Foreign Key: One(SecuritySession) To Many(RefreshTokens)
    public Guid SecuritySessionId { get; private set; }
    
    
    public string Token { get; private set; } = string.Empty;
    public bool IsRevoked { get; private set; }
    public bool IsUsed { get; private set; }

    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }


    private RefreshToken()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddDays(7);
        IsRevoked = false;
    }

    public static RefreshToken Create(Guid securitySessionId, string token)
    {
        var entity = new RefreshToken()
        {
            Token = token
        };
        entity.SecuritySessionId = securitySessionId;
        return entity;
    }

    public void Revoke()
    {
        this.IsRevoked = true;
    }

    public void Use()
    {
        this.IsUsed = true;
    }

    public void SetExpiration(DateTime expiresAt)
    {
        this.ExpiresAt = expiresAt;
    }
}

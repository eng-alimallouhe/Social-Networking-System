using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Security.Entities;

public class RefreshToken : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }


    // Foreign Key: One(User) To One(RefreshToken)
    public Guid UserId { get; set; }

    public required string Token { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }


    public RefreshToken()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = CreatedAt.AddDays(7);
    }
}
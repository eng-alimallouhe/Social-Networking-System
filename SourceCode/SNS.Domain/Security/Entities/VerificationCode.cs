
namespace SNS.Domain.Security;

public class VerificationCode
{
    // Primary Key
    public Guid Id { get; set; } = Guid.NewGuid();


    // Foreign Key: One(User) To Many(VerificationCodes)
    public Guid UserId { get; set; }

    public required string Code { get; set; }
    public CodeType Type { get; set; }


    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }


    public bool IsUsed { get; set; } = false;

    // Navigation
    public User User { get; set; } = null!;
}

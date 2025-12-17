namespace SNS.Domain.Security;

public class PasswordArchive
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(User) To Many(PasswordArchives)
    public Guid UserId { get; set; }


    public required string HashedPassword { get; set; }

    // Timestamp
    public DateTime ChangedAt { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}

using SNS.Domain.Security.Enums;

namespace SNS.Domain.Security;

public class UserArchive
{
    // Primary Key
    public Guid Id { get; set; } = Guid.NewGuid();


    // Foreign Key: One(User) To Many(Archives)
    public Guid UserId { get; set; }

    public ActionType ActionType { get; set; }

    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime TimeStamp { get; set; }

    public string? Reason { get; set; }

    // Foreign Key: One(User) To Many(ActionPerformed)
    public Guid PerformedBy { get; set; }
}

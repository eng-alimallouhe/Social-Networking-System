using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Domain.Search.Documents;

public class UserDocument
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = string.Empty;
    public SupportedLanguage PreferredLanguage { get; set; }
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public UserStatus Status { get; set; }
    public bool IsVerified { get; set; }
    public int FailedLoginAttempts { get; set; }
    public bool IsMfaEnabled { get; set; }
    public CommunicationMethod DefaultCommunicationMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastLogin { get; set; }
}

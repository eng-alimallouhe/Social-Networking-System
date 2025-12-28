namespace SNS.Application.DTOs.Security;

/// <summary>
/// Represents detailed information about a specific session.
/// </summary>
public class UserSessionDetailsDto : UserSessionSummaryDto
{
    public int? DurationMinutes { get; set; }
    public DateTime? LogoutAt { get; set; }
}

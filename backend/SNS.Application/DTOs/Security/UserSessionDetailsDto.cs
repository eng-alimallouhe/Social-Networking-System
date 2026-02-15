namespace SNS.Application.DTOs.Security;

/// <summary>
/// Represents a data transfer object used to
/// provide comprehensive details about a specific user session.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client when viewing specific session history.
/// 
/// It is typically used in detailed session views or security audits.
/// </summary>
public class UserSessionDetailsDto : UserSessionSummaryDto
{
    /// <summary>
    /// Gets or sets the duration of the session in minutes.
    /// 
    /// Optional. This value is calculated for closed sessions to show how long the user was active.
    /// </summary>
    public int? DurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the session ended.
    /// 
    /// Optional. This property is null if the session is still active.
    /// </summary>
    public DateTime? LogoutAt { get; set; }
}
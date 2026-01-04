namespace SNS.Application.DTOs.Authentication.Account.Requests;

/// <summary>
/// Represents a data transfer object used to
/// re-issue a verification code for a support-initiated action.
/// 
/// This DTO is designed to transfer data between
/// the support dashboard and the application layer, ensuring the
/// correct pending update is targeted for the retry.
/// 
/// It is typically used when a user fails to receive the initial support code.
/// </summary>
public class ResendSupportCodeDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the pending update.
    /// 
    /// This value is used to look up the specific change request.
    /// </summary>
    public Guid PendingUpdateId { get; set; }

    /// <summary>
    /// Gets or sets the new identifier (e.g., Phone Number) where the code should be sent.
    /// 
    /// This value confirms the destination for the notification.
    /// </summary>
    public string NewIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }
}
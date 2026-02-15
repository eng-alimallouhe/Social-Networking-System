namespace SNS.Application.DTOs.Authentication.Account.Requests;

/// <summary>
/// Represents a data transfer object used to
/// finalize a user-initiated identifier change (e.g., changing Email or Phone).
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer, proving ownership
/// of the new identifier via a verification code.
/// 
/// It is typically used in the "Verify Change" command.
/// </summary>
public class VerifyIdentifierChangeDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the pending update context.
    /// 
    /// This value is used to validate that the verification matches the original request.
    /// </summary>
    public Guid PendingUpdateId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the new user identifier being verified.
    /// 
    /// This value is compared against the pending request to ensure consistency.
    /// </summary>
    public string UserIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the verification code (OTP).
    /// 
    /// This value is used to authorize the final update.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}
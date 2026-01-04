namespace SNS.Application.DTOs.Authentication.Account.Requests;

/// <summary>
/// Represents a data transfer object used to
/// initiate an administrative phone number reset.
/// 
/// This DTO is designed to transfer data between
/// the support interface and the application layer, allowing
/// an administrator to force a phone number update for a user.
/// 
/// It is typically used in customer support scenarios.
/// </summary>
public class SupportResetPhoneNumberDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the user account to update.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the new phone number to assign to the user.
    /// 
    /// This value is used to update the user's contact information upon verification.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the redirect URL.
    /// 
    /// This value is used to guide the user back to a specific page after they verify the change.
    /// </summary>
    public string RedirectUrl { get; set; } = string.Empty;
}
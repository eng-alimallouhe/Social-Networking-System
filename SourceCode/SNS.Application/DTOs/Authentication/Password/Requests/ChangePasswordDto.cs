namespace SNS.Application.DTOs.Authentication.Password.Requests;

/// <summary>
/// Represents a data transfer object used to
/// change the password of a currently authenticated user.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer, ensuring the user knows
/// their current credentials before allowing a change.
/// 
/// It is typically used in the "Change Password" profile settings.
/// </summary>
public class ChangePasswordDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the user changing their password.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the user's current valid password.
    /// 
    /// This value is used as a security check to prevent unauthorized changes
    /// (e.g., if a session was left open).
    /// </summary>
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new desired password.
    /// 
    /// This value is validated against complexity policies before hashing and storage.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}
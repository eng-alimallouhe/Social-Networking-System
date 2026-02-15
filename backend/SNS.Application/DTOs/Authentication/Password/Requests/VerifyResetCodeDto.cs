namespace SNS.Application.DTOs.Authentication.Password.Requests;

/// <summary>
/// Represents a data transfer object used to
/// validate a password reset code before the final reset action.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer, ensuring the code is valid
/// and has not expired before allowing the user to proceed to the "New Password" screen.
/// 
/// It is typically used as an intermediate step in the "Forgot Password" flow.
/// </summary>
public class VerifyResetCodeDto
{
    /// <summary>
    /// Gets or sets the user's identifier (Email/Phone).
    /// 
    /// This value is used to locate the account associated with the recovery attempt.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the verification code sent to the user.
    /// 
    /// This value is checked against the stored hash to verify the user's identity.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}
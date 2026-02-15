namespace SNS.Application.DTOs.Authentication.Password.Requests;

/// <summary>
/// Represents a data transfer object used to
/// finalize the password recovery process.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer to set a new password
/// using a verified One-Time Password (OTP).
/// 
/// It is typically used in the "Forgot Password" flow.
/// </summary>
public class ResetPasswordDto
{

    /// <summary>
    /// Gets or sets the user's identifier (Email/Phone).
    /// 
    /// This value is used to locate the account requesting the reset.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the new password to be applied to the account.
    /// </summary>
    public string NewPassword { get; set; } = string.Empty;
}
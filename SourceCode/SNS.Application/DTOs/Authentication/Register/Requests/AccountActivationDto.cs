namespace SNS.Application.DTOs.Authentication.Register;

/// <summary>
/// Represents a data transfer object used to
/// finalize the user registration process by validating the activation code.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in commands to activate an account.
/// </summary>
public class AccountActivationDto
{
    /// <summary>
    /// Gets or sets the phone number associated with the account.
    /// 
    /// This value is used to identify the pending user registration record.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the verification code (OTP).
    /// 
    /// This value is used to prove ownership of the phone number and authorize activation.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}
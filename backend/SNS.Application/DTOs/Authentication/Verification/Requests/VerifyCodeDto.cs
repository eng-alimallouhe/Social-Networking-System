using SNS.Domain.Security;

namespace SNS.Application.DTOs.Authentication.Verification.Requests;

/// <summary>
/// Represents a data transfer object used to
/// submit a verification code for validation.
/// 
/// This DTO is designed to transfer data between
/// the client and the verification service without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in commands to confirm a user's identity or action (e.g., phone confirmation).
/// </summary>
public class VerifyCodeDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the user (e.g., Phone Number or Email).
    /// 
    /// This value is used to locate the specific user or pending request associated with the code.
    /// </summary>
    public string UserIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the verification code (OTP).
    /// 
    /// This value is used to prove possession of the communication channel.
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of code being verified.
    /// 
    /// This value is used to ensure the code is applied to the correct context (e.g., preventing a registration code from being used for password reset).
    /// </summary>
    public CodeType CodeType { get; set; }


    public Guid? PendingUpdateId { get; set; }
}
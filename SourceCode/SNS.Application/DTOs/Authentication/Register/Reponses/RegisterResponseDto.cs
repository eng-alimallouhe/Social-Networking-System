namespace SNS.Application.DTOs.Authentication.Register.Reponses;

/// <summary>
/// Represents a data transfer object used to
/// convey the initial results of a user registration attempt.
/// 
/// This DTO is designed to transfer data between
/// the registration service and the presentation layer, providing
/// essential identifiers needed for the next step (verification).
/// 
/// It is typically used in <see cref="Result{T}"/> responses from the Register operation.
/// </summary>
public class RegisterResponseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the <see cref="User"/>.
    /// 
    /// This value is used to reference the newly created (but potentially inactive)
    /// user account in subsequent requests.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the security verification code.
    /// 
    /// Optional. This property may be populated in development/testing environments
    /// to facilitate easy verification, but is typically null in production for security.
    /// </summary>
    public string? SecurityCode { get; set; }
}
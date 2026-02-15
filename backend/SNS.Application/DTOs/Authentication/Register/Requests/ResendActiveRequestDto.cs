namespace SNS.Application.DTOs.Authentication.Register.Requests;

/// <summary>
/// Represents a data transfer object used to
/// request the re-issuance of an account activation code.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in commands when the initial code was lost or expired.
/// </summary>
public class ResendActiveRequestDto
{
    /// <summary>
    /// Gets or sets the phone number of the user requesting the code.
    /// 
    /// This value is used to locate the specific pending registration request.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
}
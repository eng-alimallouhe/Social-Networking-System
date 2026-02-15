namespace SNS.Application.DTOs.Authentication.Password.Requests;

/// <summary>
/// Represents a data transfer object used to
/// initiate the password recovery process.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer, carrying the identifier
/// needed to locate the user account.
/// 
/// It is typically used in the "Forgot Password" command.
/// </summary>
public class ForgotPasswordDto
{
    /// <summary>
    /// Gets or sets the user's identifier (Email/Phone).
    /// 
    /// This value is used to locate the account requesting the reset
    /// and determine the destination for the recovery code.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;
}
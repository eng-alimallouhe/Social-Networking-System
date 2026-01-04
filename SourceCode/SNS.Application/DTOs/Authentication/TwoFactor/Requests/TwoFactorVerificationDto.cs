namespace SNS.Application.DTOs.Authentication.TwoFactor.Requests;

/// <summary>
/// Represents a data transfer object used to
/// complete a login attempt challenged by Two-Factor Authentication.
/// 
/// This DTO is designed to transfer data between
/// the client and the security service to validate the second factor.
/// 
/// It is typically used after the initial password validation succeeds
/// but the account requires additional verification.
/// </summary>
public class TwoFactorVerificationDto
{
    /// <summary>
    /// Gets or sets the user identifier of the user attempting to log in.
    /// </summary>
    public string UserIdentifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the 2FA code provided by the user (SMS or Authenticator App).
    /// 
    /// This value is used to finalize the authentication process and issue tokens.
    /// </summary>
    public string Code { get; set; } = string.Empty;
}
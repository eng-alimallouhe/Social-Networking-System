namespace SNS.Application.DTOs.Authentication.Account.Requests;

/// <summary>
/// Represents a data transfer object used to
/// recover access to an account using a backup security code.
/// 
/// This DTO is designed to transfer data between
/// the client and the authentication service during emergency recovery flows
/// when standard credentials or 2FA methods are unavailable.
/// 
/// It is typically used in the "Recover Account" command.
/// </summary>
public class RecoveryAccountBySecurityCodeDto
{
    /// <summary>
    /// Gets or sets the emergency security code.
    /// 
    /// This value is used to authenticate the user and bypass standard login checks.
    /// </summary>
    public string SecurityCode { get; set; } = string.Empty;
}
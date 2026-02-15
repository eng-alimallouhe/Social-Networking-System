
namespace SNS.Application.DTOs.Authentication.Common.Responses;

/// <summary>
/// Represents a data transfer object used to
/// return authentication credentials after a successful login or registration.
/// 
/// This DTO is designed to transfer data between
/// the authentication service and the client without exposing
/// internal session details or database entities.
/// 
/// It is typically used in responses for login, registration, and token refresh operations.
/// </summary>
public class AuthTokensDto
{
    /// <summary>
    /// Gets or sets the JWT Access Token.
    /// 
    /// This value is used to authenticate subsequent API requests via the Authorization header.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Refresh Token.
    /// 
    /// This value is used to obtain a new Access Token when the current one expires
    /// without requiring the user to log in again.
    /// </summary>
    public string RefreshToken { get; set; } = string.Empty;
}
namespace SNS.Application.DTOs.Authentication.Login.Requests;

/// <summary>
/// Represents a data transfer object used to
/// capture user credentials for authentication.
/// 
/// This DTO is designed to transfer data between
/// the client (login form) and the authentication service,
/// carrying the necessary information to establish a user session.
/// 
/// It is typically used in the Login command.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Gets or sets the user's unique handle (e.g., Email, Phone Number, or Username).
    /// 
    /// This value is used to look up the user account in the identity store.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's password.
    /// 
    /// This value is used to verify the user's identity against the stored hash.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
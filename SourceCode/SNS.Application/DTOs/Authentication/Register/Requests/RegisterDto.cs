namespace SNS.Application.DTOs.Authentication.Register;

/// <summary>
/// Represents a data transfer object used to
/// initiate a new user registration.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in commands to create a new user account.
/// </summary>
public class RegisterDto
{
<<<<<<< Updated upstream
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
=======
    /// <summary>
    /// Gets or sets the raw password chosen by the user.
    /// 
    /// This value is used to establish the initial security credentials 
    /// (it will be hashed before storage).
    /// </summary>
>>>>>>> Stashed changes
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's phone number.
    /// 
    /// This value is used as the primary identifier for the new account.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;
}
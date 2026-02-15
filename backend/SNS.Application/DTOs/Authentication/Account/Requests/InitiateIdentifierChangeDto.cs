namespace SNS.Application.DTOs.Authentication.Account.Requests;

/// <summary>
/// Represents a data transfer object used to
/// initiate the process of changing a user's critical identifier (Email or Phone).
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer, carrying the new identifier
/// that needs to be verified before the update is finalized.
/// 
/// It is typically used in the "Change Email" or "Change Phone" command.
/// </summary>
public class InitiateIdentifierChangeDto
{
    /// <summary>
    /// Gets or sets the new user identifier (Email or Phone Number).
    /// 
    /// This value is used to check for uniqueness and is the destination
    /// for the verification code.
    /// </summary>
    public string UserIdentifier { get; set; } = string.Empty;
}
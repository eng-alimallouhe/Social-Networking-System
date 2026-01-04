namespace SNS.Application.DTOs.Authentication.Account.Responses;

/// <summary>
/// Represents a data transfer object used to
/// return the context details after initiating an identifier change.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client, providing the necessary IDs
/// to complete the verification step.
/// 
/// It is typically used in the response of the "Initiate Identifier Change" command.
/// </summary>
public class InitiateIdentifierChangeResultDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the pending update request.
    /// 
    /// This value is required for the subsequent verification step to link the code to this specific request.
    /// </summary>
    public Guid PendingUpdateId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the new identifier (e.g., Phone Number) that was requested.
    /// 
    /// This value is echoed back to the client for confirmation or UI display.
    /// </summary>
    public string NewIdentifier { get; set; } = string.Empty;
}
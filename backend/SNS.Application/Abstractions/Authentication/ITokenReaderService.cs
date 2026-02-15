namespace SNS.Application.Abstractions.Authentication;

/// <summary>
/// Represents a domain service responsible for
/// parsing and extracting identity claims from authentication tokens.
/// 
/// This service encapsulates the business logic related to
/// token structure and claim retrieval, while keeping the Application layer
/// decoupled from infrastructure and implementation details.
/// </summary>
public interface ITokenReaderService
{
    // ------------------------------------------------------------------
    // Query operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Retrieves the email address associated with the provided access token.
    /// 
    /// This method does not mutate state and is intended for
    /// read-only scenarios.
    /// </summary>
    /// <param name="accessToken">
    /// The raw access token string to be parsed.
    /// </param>
    /// <returns>
    /// The email address if the claim exists; otherwise, null.
    /// </returns>
    string? GetEmail(string accessToken);

    /// <summary>
    /// Retrieves the unique User ID embedded within the access token.
    /// 
    /// This method does not mutate state and is intended for
    /// read-only scenarios.
    /// </summary>
    /// <param name="accessToken">
    /// The raw access token string to be parsed.
    /// </param>
    /// <returns>
    /// The unique User Identifier if the claim exists; otherwise, null.
    /// </returns>
    Guid? GetUserIdFromToken(string accessToken);

    /// <summary>
    /// Retrieves the unique Session ID embedded within the access token.
    /// 
    /// This method does not mutate state and is intended for
    /// read-only scenarios.
    /// </summary>
    /// <param name="accessToken">
    /// The raw access token string to be parsed.
    /// </param>
    /// <returns>
    /// The unique Session Identifier if the claim exists; otherwise, null.
    /// </returns>
    Guid? GetSessionIdFromToken(string accessToken);
}
using SNS.Application.Identity.Shared.DTOs.Authentication;

namespace SNS.Application.Identity.SecuritySessions.Shared.Abstractions;

/// <summary>
/// Represents a domain service responsible for
/// the generation, management, and rotation of authentication tokens.
/// 
/// This service encapsulates the business logic related to
/// JWT creation, cryptographic refresh token generation, and the secure
/// refreshing of user sessions, while keeping the Application layer
/// decoupled from infrastructure and implementation details.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a signed JWT Access Token containing user claims and session context.
    /// 
    /// This operation is responsible for:
    /// - Constructing the token payload (Claims).
    /// - Embedding the session identifier.
    /// - Signing the token with the configured security key.
    /// </summary>
    /// <param name="user">
    /// The user entity for whom the token is being generated.
    /// </param>
    /// <param name="sessionId">
    /// The unique identifier of the active session associated with this token.
    /// </param>
    /// <returns>
    /// The generated JWT access token string.
    /// </returns>
    string GenerateAccessToken(AccessTokenCreateDto dto);

}
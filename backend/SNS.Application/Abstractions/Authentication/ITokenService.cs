using SNS.Application.DTOs.Authentication.Common.Responses;
using SNS.Common.Results;
using SNS.Domain.Security.Entities;

namespace SNS.Application.Abstractions.Authentication;

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
    // ------------------------------------------------------------------
    // Command operations
    // ------------------------------------------------------------------

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
    string GenerateAccessToken(User user, Guid sessionId);

    /// <summary>
    /// Generates a cryptographically secure random string for use as a Refresh Token.
    /// 
    /// This operation is responsible for:
    /// - Creating a high-entropy random string.
    /// - Ensuring the format meets security standards for long-lived tokens.
    /// </summary>
    /// <param name="user">
    /// The user entity for whom the refresh token is being generated.
    /// </param>
    /// <returns>
    /// The generated refresh token string.
    /// </returns>
    Task<string> GrantRefreshTokenAsync(User user);

    /// <summary>
    /// Rotates the user's authentication tokens by validating existing credentials
    /// and issuing a fresh pair.
    /// 
    /// This operation is responsible for:
    /// - Validating the signature and claims of the expired access token.
    /// - Verifying the validity and ownership of the refresh token.
    /// - Revoking or rotating the used refresh token (Security rotation).
    /// - Creating a new session context.
    /// - Returning a new set of Access and Refresh tokens.
    /// </summary>
    /// <param name="oldAccessToken">
    /// The expired or expiring access token provided by the client.
    /// </param>
    /// <param name="incomingRefreshToken">
    /// The refresh token provided by the client to authorize the renewal.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthTokensDto}"/> containing the new Access and Refresh tokens
    /// if the operation completed successfully; otherwise, a failure result (e.g., Invalid Token).
    /// </returns>
    Task<Result<AuthTokensDto>> RefreshTokensAsync(string oldAccessToken, string incomingRefreshToken);

    /// <summary>
    /// Revokes all active refresh tokens associated with the specified user.
    /// 
    /// This operation is responsible for:
    /// - Identifying and removing/invalidating all persistent tokens for the user.
    /// - Forcing a "Sign Out Everywhere" scenario, requiring re-authentication on all devices.
    /// </summary>
    /// <param name="user">
    /// The user entity whose tokens are being cleared.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully.
    /// </returns>
    Task<Result> ClearRefreshTokensAsync(User user);
}
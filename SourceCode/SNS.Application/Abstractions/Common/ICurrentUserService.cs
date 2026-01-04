namespace SNS.Application.Abstractions.Common;

/// <summary>
/// Represents a domain service responsible for
/// accessing the identity and context of the currently executing user.
/// 
/// This service encapsulates the business logic related to
/// claim extraction and request context (e.g., from HTTP headers), 
/// while keeping the Application layer decoupled from infrastructure 
/// (like HttpContextAccessor).
/// </summary>
public interface ICurrentUserService
{
    // ------------------------------------------------------------------
    // Properties
    // ------------------------------------------------------------------

    /// <summary>
    /// Gets the unique identifier of the authenticated user.
    /// 
    /// This value is extracted from the authentication token or principal
    /// and serves as the primary key for user-related queries.
    /// Returns null if the request is anonymous.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the unique identifier of the active session.
    /// 
    /// This value is used to validate session-specific security constraints
    /// (e.g., forcing logout on a specific device).
    /// Returns null if the request is anonymous.
    /// </summary>
    Guid? SessionId { get; }

    /// <summary>
    /// Gets the raw JWT Bearer token from the request header.
    /// 
    /// This value is used for forwarding authentication to downstream services
    /// or for token validation operations.
    /// </summary>
    string? BearerToken { get; }

    /// <summary>
    /// Indicates whether the current request is associated with an authenticated user.
    /// 
    /// This boolean flag is a convenience wrapper checking if <see cref="UserId"/> is present.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the two-letter ISO language code (e.g., "en", "ar") for the current request.
    /// 
    /// This value is typically resolved from the "Accept-Language" header
    /// and is used to localize validation errors and response messages.
    /// </summary>
    string Language { get; }
}
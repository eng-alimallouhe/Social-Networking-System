namespace SNS.Common.StatusCodes.Security;

/// <summary>
/// Defines status codes related to user-related operations
/// and validation scenarios.
/// 
/// These status codes are used to describe the outcome of
/// user-specific workflows such as registration, lookup,
/// and existence checks.
/// </summary>
public static class UserStatusCodes
{
    /// <summary>
    /// Indicates that the requested user could not be found.
    /// </summary>
    public static readonly StatusCode UserNotFound =
        new("User", 404);

    /// <summary>
    /// Indicates that a user with the same unique identifier
    /// already exists in the system.
    /// </summary>
    public static readonly StatusCode UserAlreadyExists =
        new("User", 409);
}
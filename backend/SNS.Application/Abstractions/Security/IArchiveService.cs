using SNS.Common.Results;
using SNS.Domain.Security.Enums;

namespace SNS.Application.Abstractions.Security;

/// <summary>
/// Defines a unified service for archiving security-related events and
/// history logs within the system.
/// 
/// This service is responsible for persisting audit trails for user actions,
/// history of identity changes (email/phone), and password history to
/// enforce security policies and provide traceability.
/// </summary>
public interface IArchiveService
{
    /// <summary>
    /// Logs a specific lifecycle or security action performed by a user.
    /// 
    /// This creates an immutable record in the <c>UserArchive</c> table,
    /// detailing what happened, when, and optionally why.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user who performed the action or
    /// on whom the action was performed.
    /// </param>
    /// <param name="actionType">
    /// The specific category of the action (e.g., Login, AccountActivated).
    /// </param>
    /// <param name="performedBy">
    /// The unique identifier of the actor performing the operation. 
    /// Usually matches <paramref name="userId"/> for self-actions, but could
    /// be an Admin ID for administrative actions.
    /// </param>
    /// <param name="reason">
    /// An optional text explanation or metadata describing the reason
    /// for the action (e.g., "Suspicious activity detected").
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating the operation success.
    /// </returns>
    Task<Result> LogUserActionAsync(Guid userId, ActionType actionType, Guid performedBy, string? reason = null);

    /// <summary>
    /// Archives a historical record of a user's identity identifier
    /// (Phone Number or Email).
    /// 
    /// This is typically called immediately after a user successfully
    /// activates a new account or changes their contact information,
    /// ensuring a history of used identifiers is maintained.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="identifier">
    /// The actual value of the identifier (e.g., "+963912345678").
    /// </param>
    /// <param name="identityType">
    /// The type classification of the identifier (e.g., Phone, Email).
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating the operation success.
    /// </returns>
    Task<Result> ArchiveIdentityAsync(Guid userId, string identifier, IdentityType identityType);

    /// <summary>
    /// Archives a hashed version of the user's password.
    /// 
    /// Used to enforce password rotation policies (e.g., preventing
    /// the reuse of the last 5 passwords) and for security auditing.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user.
    /// </param>
    /// <param name="hashedPassword">
    /// The secured hash of the password to be archived.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating the operation success.
    /// </returns>
    Task<Result> ArchivePasswordAsync(Guid userId, string hashedPassword);
}
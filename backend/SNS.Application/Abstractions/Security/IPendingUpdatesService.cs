using SNS.Common.Results;
using SNS.Domain.Security.Entities;
using SNS.Domain.Security.Enums;

namespace SNS.Application.Abstractions.Security;

/// <summary>
/// Represents a domain service responsible for
/// managing the lifecycle of pending update requests (e.g., Registration, Phone Change).
/// 
/// This service encapsulates the business logic related to
/// staging area management, ensuring that conflicting updates are handled correctly 
/// (Create-or-Replace strategy) and allowing retrieval of pending data during verification flows, 
/// while keeping the Application layer decoupled from infrastructure and implementation details.
/// </summary>
public interface IPendingUpdatesService
{
    // ------------------------------------------------------------------
    // Command operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Creates a new pending update record. If a pending update of the
    /// same type already exists for the user, it is deleted and replaced
    /// to prevent stale or conflicting requests.
    /// 
    /// This operation is responsible for:
    /// - Checking for existing pending requests of the same type.
    /// - Removing obsolete requests to maintain a clean state.
    /// - Persisting the new update payload (e.g., new phone number).
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user initiating the update.
    /// </param>
    /// <param name="type">
    /// The category of the update (e.g., <see cref="UpdateType.NewRegistration"/>).
    /// </param>
    /// <param name="info">
    /// The new data payload (e.g., the new phone number or serialized JSON data).
    /// </param>
    /// <param name="supportId">
    /// Optional. The identifier of the support agent if this update was initiated by an admin.
    /// </param>
    /// <returns>
    /// A <see cref="Result{Guid}"/> containing the unique identifier of the newly created
    /// pending update record if the operation completed successfully.
    /// </returns>
    Task<Result<Guid>> CreateOrReplaceAsync(Guid userId, UpdateType type, string info, Guid? supportId = null);

    /// <summary>
    /// Permanently removes a pending update record.
    /// 
    /// This operation is responsible for:
    /// - Cleaning up the staging area after an update is successfully applied.
    /// - Removing requests that have been cancelled by the user.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the pending update to delete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating whether the operation
    /// completed successfully.
    /// </returns>
    Task<Result> DeleteAsync(Guid id);

    // ------------------------------------------------------------------
    // Query operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Retrieves a pending update based on the updated information payload.
    /// 
    /// This method does not mutate state and is intended for
    /// read-only scenarios, typically during anonymous flows (e.g., Registration)
    /// where the User ID is not yet known.
    /// </summary>
    /// <param name="info">
    /// The value to search for (e.g., the phone number).
    /// </param>
    /// <param name="type">
    /// The expected type of the update.
    /// </param>
    /// <returns>
    /// A <see cref="Result{PendingUpdate}"/> containing the entity
    /// if found; otherwise, an appropriate failure result.
    /// </returns>
    Task<Result<PendingUpdate>> GetPendingByInfoAsync(string info, UpdateType type);

    /// <summary>
    /// Retrieves a pending update for a specific user and type.
    /// 
    /// This method does not mutate state and is intended for
    /// read-only scenarios in authenticated flows (e.g., Change Password, Change Phone).
    /// </summary>
    /// <param name="userId">
    /// The user's unique identifier.
    /// </param>
    /// <param name="type">
    /// The type of update to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="Result{PendingUpdate}"/> containing the entity
    /// if found; otherwise, an appropriate failure result.
    /// </returns>
    Task<Result<PendingUpdate>> GetPendingByUserAsync(Guid userId, UpdateType type);
}
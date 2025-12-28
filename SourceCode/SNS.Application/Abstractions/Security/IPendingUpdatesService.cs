using SNS.Common.Results;
using SNS.Domain.Security.Entities;
using SNS.Domain.Security.Enums;

namespace SNS.Application.Abstractions.Security;

/// <summary>
/// Defines a service responsible for managing the lifecycle of
/// pending update requests (e.g., Registration, Phone Change).
/// 
/// This service acts as a staging area manager, ensuring that 
/// conflicting updates are handled correctly (Create-or-Replace strategy)
/// and allowing retrieval of pending data during verification flows.
/// </summary>
public interface IPendingUpdatesService
{
    /// <summary>
    /// Creates a new pending update record. If a pending update of the
    /// same type already exists for the user, it is deleted and replaced
    /// to prevent stale or conflicting requests.
    /// </summary>
    /// <param name="userId">
    /// The unique identifier of the user initiating the update.
    /// </param>
    /// <param name="type">
    /// The category of the update (e.g., <see cref="UpdateType.NewRegistration"/>).
    /// </param>
    /// <param name="info">
    /// The new data payload (e.g., the new phone number or JSON data).
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the ID of the newly created
    /// pending update record.
    /// </returns>
    Task<Result<Guid>> CreateOrReplaceAsync(Guid userId, UpdateType type, string info);

    /// <summary>
    /// Retrieves a pending update based on the updated information payload.
    /// 
    /// This is typically used during new user registration where the
    /// user ID is not yet known or authenticated, so we look up the
    /// request using the phone number or email (the "info").
    /// </summary>
    /// <param name="info">
    /// The value to search for (e.g., the phone number).
    /// </param>
    /// <param name="type">
    /// The expected type of the update.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the <see cref="PendingUpdate"/>
    /// entity if found.
    /// </returns>
    Task<Result<PendingUpdate>> GetPendingByInfoAsync(string info, UpdateType type);

    /// <summary>
    /// Retrieves a pending update for a specific user and type.
    /// 
    /// Used for authenticated flows like "Change Password" or "Change Phone",
    /// where the user ID is known from the session.
    /// </summary>
    /// <param name="userId">
    /// The user's unique identifier.
    /// </param>
    /// <param name="type">
    /// The type of update to retrieve.
    /// </param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the <see cref="PendingUpdate"/>
    /// entity if found.
    /// </returns>
    Task<Result<PendingUpdate>> GetPendingByUserAsync(Guid userId, UpdateType type);

    /// <summary>
    /// Permanently removes a pending update record.
    /// 
    /// This is called after the update has been successfully applied
    /// to the user's account, or if the user cancels the request.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the pending update to delete.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating the success of the deletion.
    /// </returns>
    Task<Result> DeleteAsync(Guid id);
}
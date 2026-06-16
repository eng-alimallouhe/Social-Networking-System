using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Shared.Results;

namespace SNS.Application.Identity.ArchiveManagement.Abstractions;

/// <summary>
/// Provides methods for archiving sensitive security-related data and logging user actions.
/// </summary>
public interface IArchiveService
{
    /// <summary>
    /// Logs a specific action performed on or by a user for audit purposes.
    /// </summary>
    Task<Result> LogUserActionAsync(CreateUserArchiveDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives a user's previous identity information (e.g., old email or phone number).
    /// </summary>
    Task<Result> ArchiveIdentityAsync(CreateIdentityArchiveDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives a user's previous password hash.
    /// </summary>
    Task<Result> ArchivePasswordAsync(Guid userId, CancellationToken cancellationToken = default);
}

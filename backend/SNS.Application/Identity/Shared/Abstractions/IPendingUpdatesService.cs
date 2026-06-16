using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Shared.Results;

namespace SNS.Application.Identity.Shared.Abstractions;

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
    Task<Result> CreateEmailUpdateAsync(CreateEmailUpdateDto dto, CancellationToken cancellationToken = default);
    Task<Result> CreatePasswordUpdateAsync(CreatePasswordUpdateDto dto, CancellationToken cancellationToken = default);

    Task<EmailUpdateModel?> GetEmailUpdateAsync(Guid userId, CancellationToken cancellationToken);
    Task<PasswordUpdateModel?> GetPasswordUpdateAsync(Guid userId, CancellationToken cancellationToken);

    Task<Result> DeleteEmailUpdateAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> DeletePasswordUpdateAsync(Guid userId, CancellationToken cancellationToken = default);
    

    Task<Result> ConfirmPasswordUpdateAsync(VerifiedPasswordUpdateDto dto, CancellationToken cancellationToken = default);
}

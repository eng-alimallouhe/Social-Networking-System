namespace SNS.Application.Identity.Shared.DTOs.PendingUpdates;

/// <summary>
/// Represents response DTO returned when initiating identifier (email/recovery email) change request.
/// </summary>
/// <param name="UserId">The unique identifier of the user requesting the change.</param>
/// <param name="Token">The verification token generated for the change request.</param>
/// <param name="CodeExpiryDate">The timestamp when the verification code expires.</param>
public sealed record IdentifierChangeResponseDto(
    Guid UserId,
    string Token,
    DateTime CodeExpiryDate);


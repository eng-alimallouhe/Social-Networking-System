namespace SNS.Application.Identity.Shared.DTOs.PendingUpdates;

public sealed record VerifiedPasswordUpdateDto(
    Guid UserId,
    string Token,
    bool IsVerified = true);

namespace SNS.Application.Identity.Shared.DTOs.PendingUpdates;

public sealed record CreatePasswordUpdateDto(
    Guid UserId,
    string Token,
    bool IsVerified = false);

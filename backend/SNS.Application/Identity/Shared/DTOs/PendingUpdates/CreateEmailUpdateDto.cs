namespace SNS.Application.Identity.Shared.DTOs.PendingUpdates;

public sealed record CreateEmailUpdateDto(
    Guid UserId,
    string NewEmail,
    string Token);

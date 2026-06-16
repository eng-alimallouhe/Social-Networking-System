namespace SNS.Application.Identity.Shared.DTOs.PendingUpdates;

public sealed record IdentifierChangeResponseDto(
    Guid UserId,
    string Token,
    DateTime CodeExpiryDate);

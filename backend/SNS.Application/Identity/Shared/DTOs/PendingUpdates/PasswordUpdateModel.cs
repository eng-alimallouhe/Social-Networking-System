namespace SNS.Application.Identity.Shared.DTOs.PendingUpdates;

public sealed record PasswordUpdateModel(
    string Token,
    bool IsVerified = true);

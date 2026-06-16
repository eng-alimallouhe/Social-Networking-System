namespace SNS.Application.Identity.Shared.DTOs.PendingUpdates;

public sealed record EmailUpdateModel(
    string NewEmail,
    string Token);

namespace SNS.Application.Identity.SecuritySettings.Queries.GetUserPasskeys;

public record PasskeyDto(
    Guid Id,
    string CredentialId,
    string DeviceName,
    DateTime CreatedAt
);

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Queries.GetUserPasskeys;

public record PasskeyDto(
    Guid Id,
    string CredentialId,
    string DeviceName,
    DateTime CreatedAt
);

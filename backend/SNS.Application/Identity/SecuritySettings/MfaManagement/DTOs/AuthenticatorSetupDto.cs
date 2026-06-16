namespace SNS.Application.Identity.SecuritySettings.MfaManagement.DTOs;

public sealed record AuthenticatorSetupDto(
    string SecretKey,
    string QrCodeUri
);

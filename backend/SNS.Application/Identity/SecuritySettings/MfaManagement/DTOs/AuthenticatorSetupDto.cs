namespace SNS.Application.Identity.SecuritySettings.MfaManagement.DTOs;

/// <summary>
/// Represents response DTO returned during authenticator app registration setup.
/// </summary>
/// <param name="SecretKey">The shared secret key for TOTP authenticator app configuration.</param>
/// <param name="QrCodeUri">The QR code URI formatted for authenticator app scanning.</param>
public sealed record AuthenticatorSetupDto(
    string SecretKey,
    string QrCodeUri
);


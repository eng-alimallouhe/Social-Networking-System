using Microsoft.Extensions.Options;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Settings;

namespace SNS.Application.Identity.Shared.Services;

public class UrlGeneratoreService : IUrlGeneratorService
{

    private readonly AppSettings _appSettings;
    private readonly string _clienUrl;

    public UrlGeneratoreService(IOptions<AppSettings> options)
    {
        _appSettings = options.Value;
        _clienUrl = _appSettings.ClientUrl.TrimEnd('/');
    }

    public string GenerateAccountActivationUrl(Guid userId, string token)
        => $"{_clienUrl}/account/activate?uid={userId}&token={Uri.EscapeDataString(token)}";


    public string GenerateEmailChangeVerificationUrl(string email, string token)
        => $"{_clienUrl}/account/verify-email?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";


    public string GeneratePasswordResetUrl(Guid userId, string token)
        => $"{_clienUrl}/account/reset-password?uid={userId}&token={Uri.EscapeDataString(token)}";

    public string GenerateSecurityEventUrl(Guid userId)
        => $"{_clienUrl}/account/devices?uid={userId}";

    public string GenerateSupportUrl(string token)
        => $"{_clienUrl}/support?token={Uri.EscapeDataString(token)}";


    public string GenerateTFARedirectUrl(Guid userId, string token)
     => $"{_clienUrl}/auth/tfa-verification?uid={userId}&token={Uri.EscapeDataString(token)}";

    public string GenerateUserDeletingUrl(Guid userId, string token)
    {
        throw new NotImplementedException();
    }
}

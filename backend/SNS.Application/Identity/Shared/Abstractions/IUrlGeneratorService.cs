namespace SNS.Application.Identity.Shared.Abstractions;

public interface IUrlGeneratorService
{
    string GenerateTFARedirectUrl(Guid userId, string Toke);
    string GenerateSecurityEventUrl(Guid userId);
    string GenerateSupportUrl(string token);
    string GenerateEmailChangeVerificationUrl(string email, string token);
    string GeneratePasswordResetUrl(Guid userId, string token);
    string GenerateAccountActivationUrl(Guid userId, string token);
    string GenerateUserDeletingUrl(Guid userId, string token);
}

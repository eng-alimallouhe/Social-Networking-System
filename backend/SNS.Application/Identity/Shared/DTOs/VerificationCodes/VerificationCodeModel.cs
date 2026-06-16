namespace SNS.Application.Identity.Shared.DTOs.VerificationCodes;

public class VerificationCodeModel
{
    public string Code { get; set; } = string.Empty;
    public int CurrentAttempt { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExipresAt { get; set; }
}

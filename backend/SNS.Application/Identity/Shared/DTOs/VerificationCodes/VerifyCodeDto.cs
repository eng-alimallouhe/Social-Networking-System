using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.VerificationCodes;

/// <summary>
/// Represents a data transfer object containing verification code validation parameters.
/// </summary>
/// <param name="UserId">The unique identifier of the user submitting the code.</param>
/// <param name="Code">The security verification code (OTP).</param>
/// <param name="Token">The verification challenge token.</param>
/// <param name="CodeType">The category type of verification code being verified.</param>
public sealed record VerifyCodeDto(
    Guid UserId,
    string Code,
    string Token,
    CodeType CodeType);


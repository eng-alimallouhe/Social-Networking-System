using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.Shared.DTOs.VerificationCodes;

/// <summary>
/// Represents a data transfer object used to
/// submit a verification code for validation.
/// </summary>
/// <param name="UserIdentifier">Gets the unique identifier of the user (e.g., Phone Number or RecoveryEmail).</param>
/// <param name="Code">Gets the verification code (OTP).</param>
/// <param name="CodeType">Gets the type of code being verified.</param>
/// <param name="PendingUpdateId">Gets the unique identifier of the pending update request. Optional.</param>
public sealed record VerifyCodeDto(
    Guid UserId,
    string Code,
    string Token,
    CodeType CodeType);

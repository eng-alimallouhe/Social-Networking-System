namespace SNS.Application.Identity.Users.Registeration.DTOs;

/// <summary>
/// Represents response DTO returned upon user registration, indicating user ID, verification token, and profile completion status.
/// </summary>
/// <param name="UserId">The unique identifier of the newly registered user account.</param>
/// <param name="Token">The verification token generated for account verification.</param>
/// <param name="IsProfileCompleted">Indicates whether the user's profile setup is complete.</param>
public sealed record RegisterResponseDto(
    Guid UserId,
    string? Token = null,
    bool? IsProfileCompleted = null);


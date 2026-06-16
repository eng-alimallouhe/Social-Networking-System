namespace SNS.Application.Identity.Users.Registeration.DTOs;

/// <summary>
/// Represents a data transfer object used to
/// convey the initial results of a user registration attempt.
/// </summary>
/// <param name="UserId">Gets the unique identifier of the user. This value is used to reference the newly created user account in subsequent requests.</param>
public sealed record RegisterResponseDto(
    Guid UserId,
    string? Token = null,
    bool? IsProfileCompleted = null);

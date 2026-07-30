using Microsoft.AspNetCore.Mvc;

namespace SNS.API.DTOs.Profiles;

/// <summary>
/// Represents request DTO supplied by the client to create a new user profile.
/// </summary>
/// <param name="FullName">The client-provided full name of the user profile.</param>
/// <param name="Specialization">Optional professional specialization title.</param>
/// <param name="Bio">Optional short biography text.</param>
/// <param name="ProfilePicture">Optional profile picture avatar file upload.</param>
public sealed record CreateProfileRequest(
    string FullName,
    string? Specialization,
    string? Bio,

    [FromForm]
    IFormFile? ProfilePicture
);
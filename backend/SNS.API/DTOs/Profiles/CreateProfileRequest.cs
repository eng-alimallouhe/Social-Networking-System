using Microsoft.AspNetCore.Mvc;

namespace SNS.API.DTOs.Profiles;

public sealed record CreateProfileRequest(
    string FullName,
    string? Specialization,
    string? Bio,

    [FromForm]
    IFormFile? ProfilePicture
);
namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents the data required to create a new user profile.
/// </summary>
/// <remarks>
/// This DTO is typically used during the initial onboarding
/// or account creation process.
/// </remarks>
public class CreateProfileDto
{
    /// <summary>
    /// The full name of the profile owner.
    /// </summary>
    /// <remarks>
    /// This value is displayed publicly and can be updated later.
    /// </remarks>
    public string FullName { get; set; } = string.Empty;
}

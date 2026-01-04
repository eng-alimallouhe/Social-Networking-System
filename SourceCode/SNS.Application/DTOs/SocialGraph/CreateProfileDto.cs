namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// provide the initial data required to create a new user profile.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in commands during the initial onboarding or account creation process.
/// </summary>
public class CreateProfileDto
{
    /// <summary>
    /// Gets or sets the full name of the profile owner.
    /// 
    /// This value is used to display the user's identity publicly.
    /// </summary>
    public string FullName { get; set; } = string.Empty;
<<<<<<< Updated upstream
}
=======

    /// <summary>
    /// Gets or sets a short biography describing the profile owner.
    /// 
    /// Optional. This property may be null if the user chooses not to provide a bio initially.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the primary specialization or job title of the user.
    /// 
    /// Optional. This property may be null depending on the user's professional status.
    /// </summary>
    public string? Specialization { get; set; }
}
>>>>>>> Stashed changes

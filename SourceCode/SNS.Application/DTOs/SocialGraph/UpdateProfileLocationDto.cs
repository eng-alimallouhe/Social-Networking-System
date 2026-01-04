namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// update the geographical location of a profile.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in commands to modify the user's residence info.
/// </summary>
public class UpdateProfileLocationDto
{
    /// <summary>
    /// Gets or sets the country where the profile owner resides.
    /// 
    /// Optional. This value is used for broad geographical filtering and display.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the city where the profile owner resides.
    /// 
    /// Optional. This value is used for localizing the user's location.
    /// </summary>
    public string? City { get; set; }
}
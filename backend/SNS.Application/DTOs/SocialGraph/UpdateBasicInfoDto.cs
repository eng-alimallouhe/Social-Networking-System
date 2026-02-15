namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// update the core identity information of a profile.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer without exposing
/// domain entities or internal business logic.
/// 
/// It is typically used in commands to modify the user's basic details.
/// </summary>
public class UpdateBasicInfoDto
{
    /// <summary>
    /// Gets or sets the updated full name of the profile owner.
    /// 
    /// This value is used to replace the current display name.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the short biography.
    /// 
    /// Optional. This value is used to update the user's "About Me" section.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the primary specialization.
    /// 
    /// Optional. This value is used to update the user's professional title.
    /// </summary>
    public string? Specialization { get; set; }
}
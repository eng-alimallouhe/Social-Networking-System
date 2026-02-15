namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// provide the current social links for editing purposes.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client to populate
/// the social links section of an edit form.
/// 
/// It is typically used in queries to fetch data before an update.
/// </summary>
public class EditableProfileLinksDto
{
    /// <summary>
    /// Gets or sets the GitHub profile URL.
    /// 
    /// Optional. This value is used to pre-fill the GitHub input field.
    /// </summary>
    public string? GitHubUrl { get; set; }

    /// <summary>
    /// Gets or sets the LinkedIn profile URL.
    /// 
    /// Optional. This value is used to pre-fill the LinkedIn input field.
    /// </summary>
    public string? LinkedInUrl { get; set; }

    /// <summary>
    /// Gets or sets the X (formerly Twitter) profile URL.
    /// 
    /// Optional. This value is used to pre-fill the X input field.
    /// </summary>
    public string? XUrl { get; set; }

    /// <summary>
    /// Gets or sets the Facebook profile URL.
    /// 
    /// Optional. This value is used to pre-fill the Facebook input field.
    /// </summary>
    public string? FacebookUrl { get; set; }

    /// <summary>
    /// Gets or sets the personal website URL.
    /// 
    /// Optional. This value is used to pre-fill the website input field.
    /// </summary>
    public string? Website { get; set; }
}
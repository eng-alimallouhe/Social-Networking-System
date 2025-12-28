namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents profile links in editable form.
/// </summary>
/// <remarks>
/// Used to populate link-editing UI components.
/// </remarks>
public class EditableProfileLinksDto
{
    /// <inheritdoc cref="UpdateProfileLinksDto.GitHubUrl" />
    public string? GitHubUrl { get; set; }

    /// <inheritdoc cref="UpdateProfileLinksDto.LinkedInUrl" />
    public string? LinkedInUrl { get; set; }

    /// <inheritdoc cref="UpdateProfileLinksDto.XUrl" />
    public string? XUrl { get; set; }

    /// <inheritdoc cref="UpdateProfileLinksDto.FacebookUrl" />
    public string? FacebookUrl { get; set; }

    /// <inheritdoc cref="UpdateProfileLinksDto.Website" />
    public string? Website { get; set; }
}

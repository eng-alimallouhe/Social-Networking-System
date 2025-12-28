namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents editable basic profile information.
/// </summary>
/// <remarks>
/// Used when updating the main identity details of a profile.
/// </remarks>
public class UpdateBasicInfoDto
{
    /// <summary>
    /// The updated full name of the profile owner.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// A short biography describing the profile owner.
    /// </summary>
    /// <remarks>
    /// This field is optional and may be null.
    /// </remarks>
    public string? Bio { get; set; }

    /// <summary>
    /// Primary specialization.
    /// </summary>
    /// <remarks>
    /// This field is optional and may be null.
    /// </remarks>
    public string? Specialization { get; set; }
}

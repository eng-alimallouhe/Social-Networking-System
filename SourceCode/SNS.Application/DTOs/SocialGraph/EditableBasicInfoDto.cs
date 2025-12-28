namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents basic profile information in editable form.
/// </summary>
/// <remarks>
/// Returned when the client requests data to populate
/// an edit profile form.
/// </remarks>
public class EditableBasicInfoDto
{
    /// <summary>
    /// The current full name of the profile owner.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// The current biography text.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// The current primary specialization.
    /// </summary>
    public string? Specialization { get; set; }
}
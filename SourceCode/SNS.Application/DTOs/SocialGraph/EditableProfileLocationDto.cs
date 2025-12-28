namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents profile location information in editable form.
/// </summary>
public class EditableProfileLocationDto
{
    /// <summary>
    /// Current country value.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Current city value.
    /// </summary>
    public string? City { get; set; }
}

using SNS.Application.DTOs.Education.Responses;

namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents education information in editable and display-ready form.
/// </summary>
public class EditableProfileEducationDto
{
    /// <summary>
    /// include university name and identifier.
    /// </summary>
    public UniversityLookupDto? University { get; set; }

    /// <summary>
    /// include Faculty name and identifier..
    /// </summary>
    public FacultyLookupDto? Faculty { get; set; }
}
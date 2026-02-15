using SNS.Application.DTOs.Education.Responses;

namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// provide the current education information for editing purposes.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client to populate
/// the education section of an edit form.
/// 
/// It is typically used in queries to fetch data before an update.
/// </summary>
public class EditableProfileEducationDto
{
    /// <summary>
    /// Gets or sets the selected university.
    /// 
    /// This value is used to display the currently selected university in the dropdown or label.
    /// </summary>
    public UniversityLookupDto? University { get; set; }

    /// <summary>
    /// Gets or sets the selected faculty.
    /// 
    /// This value is used to display the currently selected faculty in the dropdown or label.
    /// </summary>
    public FacultyLookupDto? Faculty { get; set; }
}
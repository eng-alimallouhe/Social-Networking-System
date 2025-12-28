namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents education-related profile update data.
/// </summary>
/// <remarks>
/// Used when modifying academic background information.
/// </remarks>
public class UpdateProfileEducationDto
{
    /// <summary>
    /// Identifier of the university.
    /// </summary>
    public Guid? UniversityId { get; set; }

    /// <summary>
    /// Identifier of the faculty.
    /// </summary>
    public Guid? FacultyId { get; set; }
}

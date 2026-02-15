namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// update the education background of a profile.
/// 
/// This DTO is designed to transfer data between
/// the client and the application layer, carrying references
/// to existing educational entities.
/// 
/// It is typically used in commands to modify academic information.
/// </summary>
public class UpdateProfileEducationDto
{
    /// <summary>
    /// Gets or sets the identifier of the university.
    /// 
    /// Optional. This value is used to link the profile to a specific university entity.
    /// </summary>
    public Guid? UniversityId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the faculty.
    /// 
    /// Optional. This value is used to link the profile to a specific faculty entity.
    /// </summary>
    public Guid? FacultyId { get; set; }
}
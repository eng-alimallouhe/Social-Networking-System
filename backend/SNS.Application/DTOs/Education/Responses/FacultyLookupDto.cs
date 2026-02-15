namespace SNS.Application.DTOs.Education.Responses;

/// <summary>
/// Represents a data transfer object used to
/// provide a lightweight representation of a faculty.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client without exposing
/// full faculty entities or complex relationships.
/// 
/// It is typically used in dropdowns, lists, or lookup scenarios.
/// </summary>
public class FacultyLookupDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the Faculty.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Faculty.
    /// 
    /// This value is used to display the faculty name to the user.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
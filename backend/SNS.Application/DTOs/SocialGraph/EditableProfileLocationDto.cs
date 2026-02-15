namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// provide the current location information for editing purposes.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client to populate
/// the location section of an edit form.
/// 
/// It is typically used in queries to fetch data before an update.
/// </summary>
public class EditableProfileLocationDto
{
    /// <summary>
    /// Gets or sets the current country.
    /// 
    /// Optional. This value is used to pre-fill the country selector.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets or sets the current city.
    /// 
    /// Optional. This value is used to pre-fill the city input field.
    /// </summary>
    public string? City { get; set; }
}
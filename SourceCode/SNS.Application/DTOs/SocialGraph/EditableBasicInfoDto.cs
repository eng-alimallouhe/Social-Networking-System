namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// provide the current basic profile information for editing purposes.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client to populate
/// an edit form without exposing the full profile entity.
/// 
/// It is typically used in queries to fetch data before an update.
/// </summary>
public class EditableBasicInfoDto
{
    /// <summary>
    /// Gets or sets the current full name of the profile owner.
    /// 
    /// This value is used to pre-fill the name field in the edit form.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current biography text.
    /// 
    /// Optional. This value is used to pre-fill the bio field in the edit form.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the current primary specialization.
    /// 
    /// Optional. This value is used to pre-fill the specialization field in the edit form.
    /// </summary>
    public string? Specialization { get; set; }
}
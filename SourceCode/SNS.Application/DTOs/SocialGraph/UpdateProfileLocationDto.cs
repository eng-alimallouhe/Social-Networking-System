namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents profile location update data.
/// </summary>
public class UpdateProfileLocationDto
{
    /// <summary>
    /// Country where the profile owner resides.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// City where the profile owner resides.
    /// </summary>
    public string? City { get; set; }
}
namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a lightweight, searchable snapshot of a user profile.
/// </summary>
/// <remarks>
/// Used in search results, followers lists, and suggestions.
/// </remarks>
public class ProfileSummaryDto
{
    /// <summary>
    /// Unique profile identifier.
    /// </summary>
    public Guid ProfileId { get; set; }

    /// <summary>
    /// Full name of the profile owner.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Profile picture URL.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Primary specialization or title.
    /// </summary>
    public string? Specialization { get; set; }

    /// <summary>
    /// Number of followers.
    /// </summary>
    public int FollowersCount { get; set; }

    /// <summary>
    /// Number of followed profiles.
    /// </summary>
    public int FollowingCount { get; set; }

    /// <summary>
    /// Indicates whether the viewer follows this profile.
    /// </summary>
    public bool IsFollowedByViewer { get; set; }

    /// <summary>
    /// Indicates whether the profile is blocked by the viewer.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Indicates whether the profile owner blocks the viewer.
    /// </summary>
    public bool IsBlockingViewer { get; set; }
}
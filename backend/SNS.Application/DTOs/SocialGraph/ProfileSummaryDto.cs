namespace SNS.Application.DTOs.SocialGraph;

/// <summary>
/// Represents a data transfer object used to
/// provide a lightweight snapshot of a user profile.
/// 
/// This DTO is designed to transfer data between
/// the application layer and the client for lists and search results,
/// avoiding the overhead of loading full profile details.
/// 
/// It is typically used in search results, follower lists, and suggestion cards.
/// </summary>
public class ProfileSummaryDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the profile.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the full name of the profile owner.
    /// 
    /// This value is used to display the user's identity in the list item.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL of the profile picture.
    /// 
    /// Optional. This value is used to render the user's avatar thumbnail.
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Gets or sets the primary specialization or title.
    /// 
    /// Optional. This value is used to provide context about the user's role.
    /// </summary>
    public string? Specialization { get; set; }

    /// <summary>
    /// Gets or sets the number of followers.
    /// 
    /// This value is used to indicate social proof or popularity.
    /// </summary>
    public int FollowersCount { get; set; }

    /// <summary>
    /// Gets or sets the number of profiles this user is following.
    /// </summary>
    public int FollowingCount { get; set; }

    /// <summary>
    /// Indicates whether the current viewer follows this profile.
    /// 
    /// This value is used to set the initial state of the "Follow" button in the UI.
    /// </summary>
    public bool IsFollowedByViewer { get; set; }

    /// <summary>
    /// Indicates whether the profile is blocked by the viewer.
    /// 
    /// This value is used to hide or visually distinct the profile in lists.
    /// </summary>
    public bool IsBlocked { get; set; }

    /// <summary>
    /// Indicates whether the profile owner blocks the viewer.
    /// 
    /// This value is used to restrict interactions.
    /// </summary>
    public bool IsBlockingViewer { get; set; }
}
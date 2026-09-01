namespace SNS.Application.ContentManagement.Posts.PostMentions.Contracts;

/// <summary>
/// Represents data transfer object containing mention details in a post.
/// </summary>
/// <param name="ProfileId">The unique identifier of the mentioned profile.</param>
/// <param name="DisplayName">The current full name / display name of the mentioned profile.</param>
/// <param name="ProfilePictureUrl">The resolved temporary profile picture URL of the mentioned profile.</param>
public sealed record PostMentionDto(
    Guid ProfileId,
    string DisplayName,
    string? ProfilePictureUrl
);

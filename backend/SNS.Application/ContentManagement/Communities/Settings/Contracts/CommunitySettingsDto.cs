namespace SNS.Application.ContentManagement.Communities.Settings.Contracts;

/// <summary>
/// Represents the configuration and policy settings for a community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="AllowPostWithoutApproval">Indicates whether posts can be published directly without moderator approval.</param>
/// <param name="AllowInvitationsByMembers">Indicates whether regular members can invite other users.</param>
/// <param name="AllowComments">Indicates whether comments are enabled on community posts.</param>
/// <param name="AllowMediaUpload">Indicates whether media file uploads are permitted in posts.</param>
public sealed record CommunitySettingsDto(
    Guid CommunityId,
    bool AllowPostWithoutApproval = true,
    bool AllowInvitationsByMembers = true,
    bool AllowComments = true,
    bool AllowMediaUpload = true
);

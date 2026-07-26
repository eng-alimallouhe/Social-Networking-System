namespace SNS.Application.Profiles.SocialGraph.Contracts;

public sealed record ProfileFollowDto(
    Guid ProfileId,
    string FullName,
    string? Specialization, 
    string? ProfilePictureUrl,
    DateTime FollowDate);

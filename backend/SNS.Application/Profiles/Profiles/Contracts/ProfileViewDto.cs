namespace SNS.Application.Profiles.Profiles.Contracts;

public sealed record ProfileViewDto(
    Guid ProfileId,
    string FullName,
    string? ProfilePictureUrl,
    string? Specialization,
    DateTime ViewedAt
);

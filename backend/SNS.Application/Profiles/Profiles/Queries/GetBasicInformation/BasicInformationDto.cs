namespace SNS.Application.Profiles.Profiles.Queries.GetBasicInformation;

public sealed record BasicInformationDto(
    string FullName,
    string? Bio,
    string? ProfilePictureUrl,
    string? Specialization,
    int Reputation
);

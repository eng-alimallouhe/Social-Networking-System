namespace SNS.Application.Profiles.Profiles.Contracts;

public sealed record ProfileBaseDto(
    Guid Id,
    string FullName,
    string Specialization,
    string ProfilePictureUrl,
    int Reputation);

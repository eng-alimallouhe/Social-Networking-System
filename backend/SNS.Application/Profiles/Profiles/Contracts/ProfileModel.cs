namespace SNS.Application.Profiles.Profiles.Contracts;

public sealed record ProfileIntegrationModel(
    Guid ProfileId,
    string FullName,
    Guid UserId,
    string? ProfilePictureUrl,
    string? Specialization,
    bool IsActive);

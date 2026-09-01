namespace SNS.Application.Projects.Contracts;

public sealed record ProjectParticipantDto(
    Guid ProfileId,
    string? ProfileImageUrl
);

using SNS.Domain.Projects.Enums;

namespace SNS.Application.Projects.Contracts;

public sealed record ProjectOverviewDto(
    Guid ProjectId,
    string Title,
    string ShortDescription,
    ProjectType Type,
    ProjectStatus Status,
    int ParticipantsCount,
    List<ProjectParticipantDto> Participants,
    int SkillsCount,
    List<ProjectSkillDto> Skills,
    int RatingsCount,
    double AverageRating,
    string? GitHubUrl,
    string? LiveDemoUrl
);
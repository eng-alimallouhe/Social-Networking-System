using SNS.Domain.Projects.Enums;

namespace SNS.Application.Projects.Contracts;

public record ProjectDetailsDto(
    Guid ProjectId,
    string Title,
    string ShortDescription,
    string MainImageUrl,
    string ReadmeContent,
    string GitHubUrl,
    string LiveDemoUrl,
    ProjectType Type,
    ProjectStatus Status,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<ProjectSkillDto> Skills,
    List<ProjectTagDto> Tags,
    int SaveCount,
    int ViewCount
);

public record ProjectSkillDto(Guid SkillId, string SkillName);
public record ProjectTagDto(Guid TagId, string TagName);

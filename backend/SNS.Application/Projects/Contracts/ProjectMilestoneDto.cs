namespace SNS.Application.Projects.Contracts;

public record ProjectMilestoneDto(
    Guid MilestoneId,
    string Title,
    string Description,
    DateTime CreatedAt
);

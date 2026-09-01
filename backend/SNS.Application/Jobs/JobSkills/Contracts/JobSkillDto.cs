namespace SNS.Application.Jobs.JobSkills.Contracts;

public sealed record JobSkillDto(
    Guid Id,
    Guid JobId,
    Guid SkillId,
    string SkillName
);

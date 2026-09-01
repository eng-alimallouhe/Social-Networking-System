using SNS.Application.Preferences.Skills.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Preferences.Skills.Queries.GetSkills;

public sealed record GetSkillsQuery(
    string? Search = null
) : IQuery<List<SkillDto>>;

using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Resumes.Skills.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Resumes.Skills.Queries.GetResumeSkills;

/// <summary>
/// Represents a query to retrieve all skill entries for a specific resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
public sealed record GetResumeSkillsQuery(Guid ResumeId) : IQuery<List<ResumeSkillDto>>;

/// <summary>
/// Handles the execution of <see cref="GetResumeSkillsQuery"/> to fetch skill records.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries skill records associated with the specified resume identifier.
/// 2. Projects records directly to <see cref="ResumeSkillDto"/> list.
/// </remarks>
internal sealed class GetResumeSkillsQueryHandler : IQueryHandler<GetResumeSkillsQuery, List<ResumeSkillDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetResumeSkillsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ResumeSkillDto>>> Handle(GetResumeSkillsQuery request, CancellationToken cancellationToken)
    {
        var skills = await _dbContext.ResumeSkills
            .Where(s => s.ResumeId == request.ResumeId)
            .Select(s => new ResumeSkillDto(
                s.Id,
                s.ResumeId,
                s.SkillName,
                s.Level
            ))
            .ToListAsync(cancellationToken);

        return Result<List<ResumeSkillDto>>.Success(skills, ResourceStatusCode.Found);
    }
}

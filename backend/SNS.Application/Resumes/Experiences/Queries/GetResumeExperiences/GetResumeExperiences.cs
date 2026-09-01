using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Resumes.Experiences.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Resumes.Experiences.Queries.GetResumeExperiences;

/// <summary>
/// Represents a query to retrieve all work experience history entries for a specific resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
public sealed record GetResumeExperiencesQuery(Guid ResumeId) : IQuery<List<ResumeExperienceDto>>;

/// <summary>
/// Handles the execution of <see cref="GetResumeExperiencesQuery"/> to fetch work experience entries.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries experience records associated with the specified resume identifier.
/// 2. Projects records directly to <see cref="ResumeExperienceDto"/> list.
/// </remarks>
internal sealed class GetResumeExperiencesQueryHandler : IQueryHandler<GetResumeExperiencesQuery, List<ResumeExperienceDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetResumeExperiencesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ResumeExperienceDto>>> Handle(GetResumeExperiencesQuery request, CancellationToken cancellationToken)
    {
        var experiences = await _dbContext.ResumeExperiences
            .Where(e => e.ResumeId == request.ResumeId)
            .OrderBy(e => e.StartDate)
            .Select(e => new ResumeExperienceDto(
                e.Id,
                e.ResumeId,
                e.CompanyName,
                e.Position,
                e.Description,
                e.StartDate,
                e.EndDate
            ))
            .ToListAsync(cancellationToken);

        return Result<List<ResumeExperienceDto>>.Success(experiences, ResourceStatusCode.Found);
    }
}

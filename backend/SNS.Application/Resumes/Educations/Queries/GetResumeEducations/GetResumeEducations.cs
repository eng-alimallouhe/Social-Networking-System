using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Resumes.Educations.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Resumes.Educations.Queries.GetResumeEducations;

/// <summary>
/// Represents a query to retrieve all education history entries for a specific resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
public sealed record GetResumeEducationsQuery(Guid ResumeId) : IQuery<List<ResumeEducationDto>>;

/// <summary>
/// Handles the execution of <see cref="GetResumeEducationsQuery"/> to fetch education entries.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries education records associated with the specified resume identifier.
/// 2. Projects records directly to <see cref="ResumeEducationDto"/> list.
/// </remarks>
internal sealed class GetResumeEducationsQueryHandler : IQueryHandler<GetResumeEducationsQuery, List<ResumeEducationDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetResumeEducationsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ResumeEducationDto>>> Handle(GetResumeEducationsQuery request, CancellationToken cancellationToken)
    {
        var educations = await _dbContext.ResumeEducations
            .Where(e => e.ResumeId == request.ResumeId)
            .OrderBy(e => e.StartDate)
            .Select(e => new ResumeEducationDto(
                e.Id,
                e.ResumeId,
                e.UniversityName,
                e.FacultyName,
                e.Degree,
                e.FieldOfStudy,
                e.StartDate,
                e.EndDate,
                e.GPA
            ))
            .ToListAsync(cancellationToken);

        return Result<List<ResumeEducationDto>>.Success(educations, ResourceStatusCode.Found);
    }
}

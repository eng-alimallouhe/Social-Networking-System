using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Resumes.Languages.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Resumes.Languages.Queries.GetResumeLanguages;

/// <summary>
/// Represents a query to retrieve all language proficiencies for a specific resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
public sealed record GetResumeLanguagesQuery(Guid ResumeId) : IQuery<List<ResumeLanguageDto>>;

/// <summary>
/// Handles the execution of <see cref="GetResumeLanguagesQuery"/> to fetch language proficiency records.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries language records associated with the specified resume identifier.
/// 2. Projects records directly to <see cref="ResumeLanguageDto"/> list.
/// </remarks>
internal sealed class GetResumeLanguagesQueryHandler : IQueryHandler<GetResumeLanguagesQuery, List<ResumeLanguageDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetResumeLanguagesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ResumeLanguageDto>>> Handle(GetResumeLanguagesQuery request, CancellationToken cancellationToken)
    {
        var languages = await _dbContext.ResumeLanguages
            .Where(l => l.ResumeId == request.ResumeId)
            .Select(l => new ResumeLanguageDto(
                l.Id,
                l.ResumeId,
                l.Language,
                l.Level
            ))
            .ToListAsync(cancellationToken);

        return Result<List<ResumeLanguageDto>>.Success(languages, ResourceStatusCode.Found);
    }
}

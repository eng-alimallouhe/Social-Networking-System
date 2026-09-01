using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Resumes.Certificates.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Resumes.Certificates.Queries.GetResumeCertificates;

/// <summary>
/// Represents a query to retrieve all professional certification entries for a specific resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
public sealed record GetResumeCertificatesQuery(Guid ResumeId) : IQuery<List<ResumeCertificateDto>>;

/// <summary>
/// Handles the execution of <see cref="GetResumeCertificatesQuery"/> to fetch certification records.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries certification records associated with the specified resume identifier.
/// 2. Projects records directly to <see cref="ResumeCertificateDto"/> list.
/// </remarks>
internal sealed class GetResumeCertificatesQueryHandler : IQueryHandler<GetResumeCertificatesQuery, List<ResumeCertificateDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetResumeCertificatesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<ResumeCertificateDto>>> Handle(GetResumeCertificatesQuery request, CancellationToken cancellationToken)
    {
        var certificates = await _dbContext.ResumeCertificates
            .Where(c => c.ResumeId == request.ResumeId)
            .OrderBy(c => c.IssueDate)
            .Select(c => new ResumeCertificateDto(
                c.Id,
                c.ResumeId,
                c.Title,
                c.Issuer,
                c.IssueDate
            ))
            .ToListAsync(cancellationToken);

        return Result<List<ResumeCertificateDto>>.Success(certificates, ResourceStatusCode.Found);
    }
}

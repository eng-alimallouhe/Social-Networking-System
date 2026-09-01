using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Resumes.Certificates.Contracts;
using SNS.Application.Resumes.Educations.Contracts;
using SNS.Application.Resumes.Experiences.Contracts;
using SNS.Application.Resumes.Languages.Contracts;
using SNS.Application.Resumes.Projects.Contracts;
using SNS.Application.Resumes.Resumes.Contracts;
using SNS.Application.Resumes.Resumes.Services;
using SNS.Application.Resumes.Skills.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Resumes.Queries.GetResumeById;

/// <summary>
/// Represents a query to retrieve the complete details of a specific resume by its unique identifier.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
public sealed record GetResumeByIdQuery(Guid ResumeId) : IQuery<ResumeDetailsDto>;

/// <summary>
/// Handles the execution of <see cref="GetResumeByIdQuery"/> to fetch full resume details including all sections.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries resume core attributes and related child collections from the database.
/// 2. Resolves temporary presigned URL for personal or synchronized profile avatar.
/// 3. Joins linked project entities to construct rich project summary cards.
/// 4. Maps all models to <see cref="ResumeDetailsDto"/>.
/// </remarks>
internal sealed class GetResumeByIdQueryHandler : IQueryHandler<GetResumeByIdQuery, ResumeDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IResumeUrlResolver _resumeUrlResolver;
    private readonly IFileStorageService _fileStorageService;

    public GetResumeByIdQueryHandler(
        IApplicationDbContext dbContext,
        IResumeUrlResolver resumeUrlResolver,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _resumeUrlResolver = resumeUrlResolver;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<ResumeDetailsDto>> Handle(GetResumeByIdQuery request, CancellationToken cancellationToken)
    {
        var rawResume = await _dbContext.Resumes
            .Where(r => r.Id == request.ResumeId && r.IsActive)
            .Select(r => new
            {
                r.Id,
                r.OwnerId,
                r.PersonalPictureUrl,
                r.SyncProfilePicture,
                r.Title,
                r.Template,
                r.Summary,
                r.Langauge,
                r.CreatedAt,
                r.UpdatedAt,
                Educations = r.Educations.Select(e => new ResumeEducationDto(
                    e.Id,
                    e.ResumeId,
                    e.UniversityName,
                    e.FacultyName,
                    e.Degree,
                    e.FieldOfStudy,
                    e.StartDate,
                    e.EndDate,
                    e.GPA
                )).ToList(),
                Experiences = r.Experiences.Select(e => new ResumeExperienceDto(
                    e.Id,
                    e.ResumeId,
                    e.CompanyName,
                    e.Position,
                    e.Description,
                    e.StartDate,
                    e.EndDate
                )).ToList(),
                Certificates = r.Certificates.Select(c => new ResumeCertificateDto(
                    c.Id,
                    c.ResumeId,
                    c.Title,
                    c.Issuer,
                    c.IssueDate
                )).ToList(),
                Languages = r.Languages.Select(l => new ResumeLanguageDto(
                    l.Id,
                    l.ResumeId,
                    l.Language,
                    l.Level
                )).ToList(),
                Skills = r.Skills.Select(s => new ResumeSkillDto(
                    s.Id,
                    s.ResumeId,
                    s.SkillName,
                    s.Level
                )).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (rawResume == null)
        {
            return Result<ResumeDetailsDto>.Failure(ResumeStatusCodes.ResumeNotFound);
        }

        // Fetch linked projects
        var rawProjects = await (
            from rp in _dbContext.ResumeProjects
            join p in _dbContext.Projects on rp.ProjectId equals p.Id
            where rp.ResumeId == request.ResumeId && p.IsActive
            select new
            {
                rp.ResumeId,
                rp.ProjectId,
                p.Title,
                p.ShortDescription,
                p.MainImageUrl,
                p.Type,
                p.Status
            }
        ).ToListAsync(cancellationToken);

        // Resolve personal picture URL
        var resolvedPictureUrl = await _resumeUrlResolver.ResolvePersonalPictureUrlAsync(
            rawResume.PersonalPictureUrl,
            rawResume.SyncProfilePicture,
            rawResume.OwnerId,
            cancellationToken
        );

        // Resolve project main image URLs if present
        var projectImageKeys = rawProjects
            .Select(p => p.MainImageUrl)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();

        Dictionary<string, string> projectImageMap = new();
        if (projectImageKeys.Any())
        {
            var imgTasks = projectImageKeys.Select(async k => new
            {
                Key = k!,
                Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
            });
            var resolved = await Task.WhenAll(imgTasks);
            projectImageMap = resolved.ToDictionary(x => x.Key, x => x.Url);
        }

        var projectDtos = rawProjects.Select(p => new ResumeProjectDto(
            p.ResumeId,
            p.ProjectId,
            p.Title,
            p.ShortDescription,
            !string.IsNullOrWhiteSpace(p.MainImageUrl) && projectImageMap.TryGetValue(p.MainImageUrl, out var imgUrl) ? imgUrl : p.MainImageUrl,
            p.Type,
            p.Status
        )).ToList();

        var detailsDto = new ResumeDetailsDto(
            rawResume.Id,
            rawResume.OwnerId,
            resolvedPictureUrl,
            rawResume.SyncProfilePicture,
            rawResume.Title,
            rawResume.Template,
            rawResume.Summary,
            rawResume.Langauge,
            rawResume.CreatedAt,
            rawResume.UpdatedAt,
            rawResume.Educations,
            rawResume.Experiences,
            rawResume.Certificates,
            rawResume.Languages,
            rawResume.Skills,
            projectDtos
        );

        return Result<ResumeDetailsDto>.Success(detailsDto, ResourceStatusCode.Found);
    }
}

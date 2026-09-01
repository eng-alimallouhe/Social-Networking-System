using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Jobs.Companies.Contracts;
using SNS.Application.Jobs.Jobs.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.Jobs.Queries.GetJobById;

public sealed record GetJobByIdQuery(Guid JobId) : IQuery<JobDetailsDto>;

internal sealed class GetJobByIdQueryHandler : IQueryHandler<GetJobByIdQuery, JobDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetJobByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<JobDetailsDto>> Handle(GetJobByIdQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;

        var jobData = await _dbContext.Jobs
            .AsNoTracking()
            .Where(j => j.Id == request.JobId && j.IsActive)
            .Select(j => new
            {
                j.Id,
                j.CompanyId,
                CompanyName = j.Company.Name,
                CompanyIndustry = j.Company.Industry,
                CompanyWebsiteUrl = j.Company.WebsiteUrl,
                CompanyLogoObjectKey = j.Company.LogoObjectKey,
                j.Title,
                j.Description,
                j.Location,
                j.Type,
                j.MinSalary,
                j.MaxSalary,
                j.CurrencyCode,
                j.SalaryType,
                j.KeyResponsibilitiesText,
                j.CreatedAt,
                j.ClosedAt,
                j.IsActive,
                ApplicationsCount = j.Applications.Count(a => a.IsActive),
                Skills = _dbContext.JobSkills
                    .Where(js => js.JobId == j.Id)
                    .Select(js => _dbContext.Skills.Where(s => s.Id == js.SkillId).Select(s => s.Name).FirstOrDefault() ?? string.Empty)
                    .Where(name => name != string.Empty)
                    .ToList(),
                HasApplied = currentProfileId.HasValue && j.Applications.Any(a => a.ApplicantId == currentProfileId.Value && a.IsActive)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (jobData == null)
        {
            return Result<JobDetailsDto>.Failure(JobStatusCodes.JobNotFound);
        }

        var companySnapshot = new CompanySnapshotDto(
            Id: jobData.CompanyId,
            Name: jobData.CompanyName,
            Industry: jobData.CompanyIndustry,
            WebsiteUrl: jobData.CompanyWebsiteUrl,
            LogoUrl: !string.IsNullOrWhiteSpace(jobData.CompanyLogoObjectKey)
                ? _fileStorageService.GetFilePublicUrl(jobData.CompanyLogoObjectKey)
                : null
        );

        var details = new JobDetailsDto(
            Id: jobData.Id,
            CompanyId: jobData.CompanyId,
            Company: companySnapshot,
            Title: jobData.Title,
            Description: jobData.Description,
            Location: jobData.Location,
            Type: jobData.Type,
            MinSalary: jobData.MinSalary,
            MaxSalary: jobData.MaxSalary,
            CurrencyCode: jobData.CurrencyCode,
            SalaryType: jobData.SalaryType,
            KeyResponsibilitiesText: jobData.KeyResponsibilitiesText,
            Skills: jobData.Skills,
            ApplicationsCount: jobData.ApplicationsCount,
            HasApplied: jobData.HasApplied,
            CreatedAt: jobData.CreatedAt,
            ClosedAt: jobData.ClosedAt,
            IsActive: jobData.IsActive
        );

        return Result<JobDetailsDto>.Success(details, OperationStatusCode.Success);
    }
}

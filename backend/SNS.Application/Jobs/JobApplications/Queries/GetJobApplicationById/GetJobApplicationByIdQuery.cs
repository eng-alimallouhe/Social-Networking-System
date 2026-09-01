using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Jobs.JobApplications.Contracts;
using SNS.Application.Jobs.Jobs.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.JobApplications.Queries.GetJobApplicationById;

public sealed record GetJobApplicationByIdQuery(Guid ApplicationId) : IQuery<JobApplicationDetailsDto>;

internal sealed class GetJobApplicationByIdQueryHandler : IQueryHandler<GetJobApplicationByIdQuery, JobApplicationDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetJobApplicationByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<JobApplicationDetailsDto>> Handle(GetJobApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<JobApplicationDetailsDto>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var raw = await _dbContext.JobApplications
            .AsNoTracking()
            .Where(a => a.Id == request.ApplicationId && a.IsActive)
            .Select(a => new
            {
                a.Id,
                a.JobId,
                a.ApplicantId,
                ApplicantFullName = _dbContext.Profiles.Where(p => p.Id == a.ApplicantId).Select(p => p.FullName).FirstOrDefault() ?? string.Empty,
                ApplicantSpecialization = _dbContext.Profiles.Where(p => p.Id == a.ApplicantId).Select(p => p.Specialization).FirstOrDefault() ?? string.Empty,
                ApplicantAvatarKey = _dbContext.Profiles.Where(p => p.Id == a.ApplicantId).Select(p => p.ProfilePictureObjectKey).FirstOrDefault(),
                a.ResumeId,
                a.CoverLetterText,
                a.ResumeFileUrl,
                a.Status,
                a.CreatedAt,
                a.UpdatedAt,
                a.IsActive,
                JobTitle = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.Title).FirstOrDefault() ?? string.Empty,
                JobCompanyId = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.CompanyId).FirstOrDefault(),
                JobCompanyName = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.Company.Name).FirstOrDefault() ?? string.Empty,
                JobLocation = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.Location).FirstOrDefault() ?? string.Empty,
                JobType = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.Type).FirstOrDefault(),
                JobMinSalary = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.MinSalary).FirstOrDefault(),
                JobMaxSalary = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.MaxSalary).FirstOrDefault(),
                JobCurrencyCode = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.CurrencyCode).FirstOrDefault() ?? string.Empty,
                JobSalaryType = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.SalaryType).FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (raw == null)
        {
            return Result<JobApplicationDetailsDto>.Failure(JobApplicationStatusCodes.ApplicationNotFound);
        }

        var isApplicant = raw.ApplicantId == currentProfileId.Value;
        var isCompanyAdmin = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == raw.JobCompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (!isApplicant && !isCompanyAdmin)
        {
            return Result<JobApplicationDetailsDto>.Failure(JobApplicationStatusCodes.NotApplicant);
        }

        var applicantSnapshot = new ProfileSnapshotDto(
            Id: raw.ApplicantId,
            FullName: raw.ApplicantFullName,
            Specialization: raw.ApplicantSpecialization,
            ProfilePictureUrl: !string.IsNullOrWhiteSpace(raw.ApplicantAvatarKey)
                ? _fileStorageService.GetFilePublicUrl(raw.ApplicantAvatarKey)
                : null
        );

        var jobSnapshot = new JobSnapshotDto(
            Id: raw.JobId,
            Title: raw.JobTitle,
            CompanyName: raw.JobCompanyName,
            Location: raw.JobLocation,
            Type: raw.JobType,
            MinSalary: raw.JobMinSalary,
            MaxSalary: raw.JobMaxSalary,
            CurrencyCode: raw.JobCurrencyCode,
            SalaryType: raw.JobSalaryType
        );

        var details = new JobApplicationDetailsDto(
            Id: raw.Id,
            JobId: raw.JobId,
            Job: jobSnapshot,
            ApplicantId: raw.ApplicantId,
            Applicant: applicantSnapshot,
            ResumeId: raw.ResumeId,
            CoverLetterText: raw.CoverLetterText,
            ResumeFileUrl: raw.ResumeFileUrl,
            Status: raw.Status,
            CreatedAt: raw.CreatedAt,
            UpdatedAt: raw.UpdatedAt,
            IsActive: raw.IsActive
        );

        return Result<JobApplicationDetailsDto>.Success(details, OperationStatusCode.Success);
    }
}

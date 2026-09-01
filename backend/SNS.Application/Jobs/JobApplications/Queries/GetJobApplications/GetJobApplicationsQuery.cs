using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Jobs.JobApplications.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.QA.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Jobs.JobApplications.Queries.GetJobApplications;

public sealed record GetJobApplicationsQuery(
    Guid? JobId = null,
    Guid? CompanyId = null,
    ApplicationStatus? Status = null,
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<JobApplicationSummaryDto>>;

internal sealed class GetJobApplicationsQueryHandler : IQueryHandler<GetJobApplicationsQuery, Paged<JobApplicationSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetJobApplicationsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<JobApplicationSummaryDto>>> Handle(GetJobApplicationsQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<Paged<JobApplicationSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var managedCompanyIds = await _dbContext.CompanyAdministrators
            .Where(ca => ca.ProfileId == currentProfileId.Value)
            .Select(ca => ca.CompanyId)
            .ToListAsync(cancellationToken);

        if (managedCompanyIds.Count == 0)
        {
            return Result<Paged<JobApplicationSummaryDto>>.Success(new Paged<JobApplicationSummaryDto>(
                items: new List<JobApplicationSummaryDto>(),
                count: 0,
                pageSize: request.PageSize,
                currentPage: request.CurrentPage), OperationStatusCode.Success);
        }

        var query = from a in _dbContext.JobApplications.AsNoTracking()
                    join j in _dbContext.Jobs.AsNoTracking() on a.JobId equals j.Id
                    where a.IsActive && managedCompanyIds.Contains(j.CompanyId)
                    select new { Application = a, Job = j };

        if (request.CompanyId.HasValue)
        {
            query = query.Where(x => x.Job.CompanyId == request.CompanyId.Value);
        }

        if (request.JobId.HasValue)
        {
            query = query.Where(x => x.Application.JobId == request.JobId.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Application.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var rawList = await query
            .OrderByDescending(x => x.Application.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new
            {
                x.Application.Id,
                x.Application.JobId,
                JobTitle = x.Job.Title,
                CompanyName = x.Job.Company.Name,
                x.Application.ApplicantId,
                ApplicantFullName = _dbContext.Profiles.Where(p => p.Id == x.Application.ApplicantId).Select(p => p.FullName).FirstOrDefault() ?? string.Empty,
                ApplicantAvatarKey = _dbContext.Profiles.Where(p => p.Id == x.Application.ApplicantId).Select(p => p.ProfilePictureObjectKey).FirstOrDefault(),
                ApplicantSpecialization = _dbContext.Profiles.Where(p => p.Id == x.Application.ApplicantId).Select(p => p.Specialization).FirstOrDefault() ?? string.Empty,
                x.Application.ResumeId,
                x.Application.ResumeFileUrl,
                x.Application.Status,
                x.Application.CreatedAt,
                x.Application.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        var items = rawList.Select(a => new JobApplicationSummaryDto(
            Id: a.Id,
            JobId: a.JobId,
            JobTitle: a.JobTitle,
            CompanyName: a.CompanyName,
            ApplicantId: a.ApplicantId,
            ApplicantFullName: a.ApplicantFullName,
            ApplicantAvatarUrl: !string.IsNullOrWhiteSpace(a.ApplicantAvatarKey)
                ? _fileStorageService.GetFilePublicUrl(a.ApplicantAvatarKey)
                : null,
            ApplicantSpecialization: a.ApplicantSpecialization,
            ResumeId: a.ResumeId,
            ResumeFileUrl: a.ResumeFileUrl,
            Status: a.Status,
            CreatedAt: a.CreatedAt,
            UpdatedAt: a.UpdatedAt
        )).ToList();

        return Result<Paged<JobApplicationSummaryDto>>.Success(new Paged<JobApplicationSummaryDto>(
            items: items,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage), OperationStatusCode.Success);
    }
}

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

namespace SNS.Application.Jobs.JobApplications.Queries.GetMyJobApplications;

public sealed record GetMyJobApplicationsQuery(
    ApplicationStatus? Status = null,
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<JobApplicationSummaryDto>>;

internal sealed class GetMyJobApplicationsQueryHandler : IQueryHandler<GetMyJobApplicationsQuery, Paged<JobApplicationSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetMyJobApplicationsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<JobApplicationSummaryDto>>> Handle(GetMyJobApplicationsQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<Paged<JobApplicationSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var query = _dbContext.JobApplications
            .AsNoTracking()
            .Where(a => a.ApplicantId == currentProfileId.Value && a.IsActive);

        if (request.Status.HasValue)
        {
            query = query.Where(a => a.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var rawList = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new
            {
                a.Id,
                a.JobId,
                JobTitle = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.Title).FirstOrDefault() ?? string.Empty,
                CompanyName = _dbContext.Jobs.Where(j => j.Id == a.JobId).Select(j => j.Company.Name).FirstOrDefault() ?? string.Empty,
                a.ApplicantId,
                ApplicantFullName = _dbContext.Profiles.Where(p => p.Id == a.ApplicantId).Select(p => p.FullName).FirstOrDefault() ?? string.Empty,
                ApplicantAvatarKey = _dbContext.Profiles.Where(p => p.Id == a.ApplicantId).Select(p => p.ProfilePictureObjectKey).FirstOrDefault(),
                ApplicantSpecialization = _dbContext.Profiles.Where(p => p.Id == a.ApplicantId).Select(p => p.Specialization).FirstOrDefault() ?? string.Empty,
                a.ResumeId,
                a.ResumeFileUrl,
                a.Status,
                a.CreatedAt,
                a.UpdatedAt
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

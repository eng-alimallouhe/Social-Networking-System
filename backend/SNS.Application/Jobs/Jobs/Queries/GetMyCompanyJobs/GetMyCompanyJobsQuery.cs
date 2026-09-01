using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Jobs.Jobs.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Jobs.Jobs.Queries.GetMyCompanyJobs;

public sealed record GetMyCompanyJobsQuery(
    Guid? CompanyId = null,
    int PageSize = 10,
    int CurrentPage = 1,
    bool IncludeClosed = true
) : IQuery<Paged<JobSummaryDto>>;

internal sealed class GetMyCompanyJobsQueryHandler : IQueryHandler<GetMyCompanyJobsQuery, Paged<JobSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetMyCompanyJobsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<JobSummaryDto>>> Handle(GetMyCompanyJobsQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<Paged<JobSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var managedCompanyIds = await _dbContext.CompanyAdministrators
            .Where(ca => ca.ProfileId == currentProfileId.Value)
            .Select(ca => ca.CompanyId)
            .ToListAsync(cancellationToken);

        if (managedCompanyIds.Count == 0)
        {
            return Result<Paged<JobSummaryDto>>.Success(new Paged<JobSummaryDto>(
                items: new List<JobSummaryDto>(),
                count: 0,
                pageSize: request.PageSize,
                currentPage: request.CurrentPage), OperationStatusCode.Success);
        }

        var query = _dbContext.Jobs
            .AsNoTracking()
            .Where(j => managedCompanyIds.Contains(j.CompanyId) && j.IsActive);

        if (request.CompanyId.HasValue)
        {
            query = query.Where(j => j.CompanyId == request.CompanyId.Value);
        }

        if (!request.IncludeClosed)
        {
            query = query.Where(j => j.ClosedAt == null);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var rawItems = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(j => new
            {
                j.Id,
                j.CompanyId,
                CompanyName = j.Company.Name,
                CompanyLogoObjectKey = j.Company.LogoObjectKey,
                j.Title,
                j.Description,
                j.Location,
                j.Type,
                j.MinSalary,
                j.MaxSalary,
                j.CurrencyCode,
                j.SalaryType,
                ApplicationsCount = j.Applications.Count(a => a.IsActive),
                Skills = _dbContext.JobSkills
                    .Where(js => js.JobId == j.Id)
                    .Select(js => _dbContext.Skills.Where(s => s.Id == js.SkillId).Select(s => s.Name).FirstOrDefault() ?? string.Empty)
                    .Where(name => name != string.Empty)
                    .ToList(),
                j.CreatedAt,
                j.ClosedAt,
                j.IsActive
            })
            .ToListAsync(cancellationToken);

        var items = rawItems.Select(j => new JobSummaryDto(
            Id: j.Id,
            CompanyId: j.CompanyId,
            CompanyName: j.CompanyName,
            CompanyLogoUrl: !string.IsNullOrWhiteSpace(j.CompanyLogoObjectKey)
                ? _fileStorageService.GetFilePublicUrl(j.CompanyLogoObjectKey)
                : null,
            Title: j.Title,
            Description: j.Description,
            Location: j.Location,
            Type: j.Type,
            MinSalary: j.MinSalary,
            MaxSalary: j.MaxSalary,
            CurrencyCode: j.CurrencyCode,
            SalaryType: j.SalaryType,
            ApplicationsCount: j.ApplicationsCount,
            Skills: j.Skills,
            CreatedAt: j.CreatedAt,
            ClosedAt: j.ClosedAt,
            IsActive: j.IsActive
        )).ToList();

        return Result<Paged<JobSummaryDto>>.Success(new Paged<JobSummaryDto>(
            items: items,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage), OperationStatusCode.Success);
    }
}

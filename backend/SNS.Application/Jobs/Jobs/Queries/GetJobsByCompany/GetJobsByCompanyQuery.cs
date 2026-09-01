using Microsoft.EntityFrameworkCore;
using SNS.Application.Jobs.Jobs.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Jobs.Jobs.Queries.GetJobsByCompany;

public sealed record GetJobsByCompanyQuery(
    Guid CompanyId,
    int PageSize = 10,
    int CurrentPage = 1,
    bool IncludeClosed = false
) : IQuery<Paged<JobSummaryDto>>;

internal sealed class GetJobsByCompanyQueryHandler : IQueryHandler<GetJobsByCompanyQuery, Paged<JobSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetJobsByCompanyQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<JobSummaryDto>>> Handle(GetJobsByCompanyQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Jobs
            .AsNoTracking()
            .Where(j => j.CompanyId == request.CompanyId && j.IsActive);

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

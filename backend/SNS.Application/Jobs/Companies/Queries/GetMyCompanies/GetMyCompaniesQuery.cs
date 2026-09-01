using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Jobs.Companies.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Jobs.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Jobs.Companies.Queries.GetMyCompanies;

public sealed record GetMyCompaniesQuery : IQuery<List<CompanySummaryDto>>;

internal sealed class GetMyCompaniesQueryHandler : IQueryHandler<GetMyCompaniesQuery, List<CompanySummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetMyCompaniesQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<List<CompanySummaryDto>>> Handle(GetMyCompaniesQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<List<CompanySummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var adminRecords = await _dbContext.CompanyAdministrators
            .AsNoTracking()
            .Where(ca => ca.ProfileId == currentProfileId.Value && ca.Company.IsActive)
            .Select(ca => new
            {
                ca.Company.Id,
                ca.Company.Name,
                ca.Company.Industry,
                ca.Company.WebsiteUrl,
                ca.Company.LogoObjectKey,
                ca.Company.CreatedAt,
                ActiveJobsCount = ca.Company.PostedJobs.Count(j => j.IsActive && j.ClosedAt == null),
                ca.AdminRole
            })
            .ToListAsync(cancellationToken);

        var items = adminRecords.Select(c => new CompanySummaryDto(
            Id: c.Id,
            Name: c.Name,
            Industry: c.Industry,
            WebsiteUrl: c.WebsiteUrl,
            LogoUrl: !string.IsNullOrWhiteSpace(c.LogoObjectKey)
                ? _fileStorageService.GetFilePublicUrl(c.LogoObjectKey)
                : null,
            CreatedAt: c.CreatedAt,
            ActiveJobsCount: c.ActiveJobsCount,
            CurrentUserRole: c.AdminRole
        )).ToList();

        return Result<List<CompanySummaryDto>>.Success(items, OperationStatusCode.Success);
    }
}

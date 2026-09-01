using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Jobs.Companies.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Jobs.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.Companies.Queries.GetCompanyById;

public sealed record GetCompanyByIdQuery(Guid CompanyId) : IQuery<CompanyDetailsDto>;

internal sealed class GetCompanyByIdQueryHandler : IQueryHandler<GetCompanyByIdQuery, CompanyDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetCompanyByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<CompanyDetailsDto>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;

        var company = await _dbContext.Companies
            .AsNoTracking()
            .Where(c => c.Id == request.CompanyId && c.IsActive)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Industry,
                c.WebsiteUrl,
                c.LogoObjectKey,
                c.CreatedAt,
                c.IsActive,
                ActiveJobsCount = c.PostedJobs.Count(j => j.IsActive && j.ClosedAt == null),
                AdministratorsCount = c.Administrators.Count(),
                CurrentUserRole = currentProfileId.HasValue
                    ? c.Administrators
                        .Where(a => a.ProfileId == currentProfileId.Value)
                        .Select(a => (CompanyRole?)a.AdminRole)
                        .FirstOrDefault()
                    : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (company == null)
        {
            return Result<CompanyDetailsDto>.Failure(CompanyStatusCodes.CompanyNotFound);
        }

        var details = new CompanyDetailsDto(
            Id: company.Id,
            Name: company.Name,
            Industry: company.Industry,
            WebsiteUrl: company.WebsiteUrl,
            LogoUrl: !string.IsNullOrWhiteSpace(company.LogoObjectKey)
                ? _fileStorageService.GetFilePublicUrl(company.LogoObjectKey)
                : null,
            CreatedAt: company.CreatedAt,
            IsActive: company.IsActive,
            ActiveJobsCount: company.ActiveJobsCount,
            AdministratorsCount: company.AdministratorsCount,
            CurrentUserRole: company.CurrentUserRole
        );

        return Result<CompanyDetailsDto>.Success(details, OperationStatusCode.Success);
    }
}

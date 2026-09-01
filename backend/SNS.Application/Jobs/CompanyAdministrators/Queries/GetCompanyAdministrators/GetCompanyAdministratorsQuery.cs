using Microsoft.EntityFrameworkCore;
using SNS.Application.Jobs.CompanyAdministrators.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.CompanyAdministrators.Queries.GetCompanyAdministrators;

public sealed record GetCompanyAdministratorsQuery(Guid CompanyId) : IQuery<List<CompanyAdministratorDto>>;

internal sealed class GetCompanyAdministratorsQueryHandler : IQueryHandler<GetCompanyAdministratorsQuery, List<CompanyAdministratorDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetCompanyAdministratorsQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<List<CompanyAdministratorDto>>> Handle(GetCompanyAdministratorsQuery request, CancellationToken cancellationToken)
    {
        var companyExists = await _dbContext.Companies
            .AnyAsync(c => c.Id == request.CompanyId && c.IsActive, cancellationToken);

        if (!companyExists)
        {
            return Result<List<CompanyAdministratorDto>>.Failure(CompanyStatusCodes.CompanyNotFound);
        }

        var rawList = await _dbContext.CompanyAdministrators
            .AsNoTracking()
            .Where(ca => ca.CompanyId == request.CompanyId && ca.Profile.IsActive)
            .Select(ca => new
            {
                ca.Id,
                ca.CompanyId,
                ca.ProfileId,
                ProfileFullName = ca.Profile.FullName,
                ProfileSpecialization = ca.Profile.Specialization,
                ProfileAvatarObjectKey = ca.Profile.ProfilePictureObjectKey,
                ca.AdminRole
            })
            .ToListAsync(cancellationToken);

        var items = rawList.Select(ca => new CompanyAdministratorDto(
            Id: ca.Id,
            CompanyId: ca.CompanyId,
            ProfileId: ca.ProfileId,
            Profile: new ProfileSnapshotDto(
                Id: ca.ProfileId,
                FullName: ca.ProfileFullName,
                Specialization: ca.ProfileSpecialization,
                ProfilePictureUrl: !string.IsNullOrWhiteSpace(ca.ProfileAvatarObjectKey)
                    ? _fileStorageService.GetFilePublicUrl(ca.ProfileAvatarObjectKey)
                    : null
            ),
            AdminRole: ca.AdminRole
        )).ToList();

        return Result<List<CompanyAdministratorDto>>.Success(items, OperationStatusCode.Success);
    }
}

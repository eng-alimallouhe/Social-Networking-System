using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Jobs.CompanyCreateRequests.Contracts;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.CompanyCreateRequests.Queries.GetCompanyCreateRequestById;

public sealed record GetCompanyCreateRequestByIdQuery(Guid RequestId) : IQuery<CompanyCreateRequestDetailsDto>;

internal sealed class GetCompanyCreateRequestByIdQueryHandler : IQueryHandler<GetCompanyCreateRequestByIdQuery, CompanyCreateRequestDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetCompanyCreateRequestByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<CompanyCreateRequestDetailsDto>> Handle(GetCompanyCreateRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<CompanyCreateRequestDetailsDto>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var raw = await _dbContext.CompanyCreateRequests
            .AsNoTracking()
            .Where(r => r.Id == request.RequestId)
            .Select(r => new
            {
                r.Id,
                r.ProfileId,
                ProfileFullName = r.Profile.FullName,
                ProfileSpecialization = r.Profile.Specialization,
                ProfileAvatarObjectKey = r.Profile.ProfilePictureObjectKey,
                r.Name,
                r.Industry,
                r.WebsiteUrl,
                r.LogoObjectKey,
                r.Status,
                r.CreatedCompanyId,
                r.ReviewedByProfileId,
                r.ReviewNote,
                r.CreatedAt,
                r.ReviewedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (raw == null)
        {
            return Result<CompanyCreateRequestDetailsDto>.Failure(CompanyCreateRequestStatusCodes.RequestNotFound);
        }

        var profileSnapshot = new ProfileSnapshotDto(
            Id: raw.ProfileId,
            FullName: raw.ProfileFullName,
            Specialization: raw.ProfileSpecialization,
            ProfilePictureUrl: !string.IsNullOrWhiteSpace(raw.ProfileAvatarObjectKey)
                ? _fileStorageService.GetFilePublicUrl(raw.ProfileAvatarObjectKey)
                : null
        );

        var details = new CompanyCreateRequestDetailsDto(
            Id: raw.Id,
            ProfileId: raw.ProfileId,
            Profile: profileSnapshot,
            Name: raw.Name,
            Industry: raw.Industry,
            WebsiteUrl: raw.WebsiteUrl,
            LogoUrl: !string.IsNullOrWhiteSpace(raw.LogoObjectKey)
                ? _fileStorageService.GetFilePublicUrl(raw.LogoObjectKey)
                : null,
            Status: raw.Status,
            CreatedCompanyId: raw.CreatedCompanyId,
            ReviewedByProfileId: raw.ReviewedByProfileId,
            ReviewNote: raw.ReviewNote,
            CreatedAt: raw.CreatedAt,
            ReviewedAt: raw.ReviewedAt
        );

        return Result<CompanyCreateRequestDetailsDto>.Success(details, OperationStatusCode.Success);
    }
}

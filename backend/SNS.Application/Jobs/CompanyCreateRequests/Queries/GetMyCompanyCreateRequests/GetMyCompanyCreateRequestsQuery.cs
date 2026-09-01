using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Jobs.CompanyCreateRequests.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Jobs.CompanyCreateRequests.Queries.GetMyCompanyCreateRequests;

public sealed record GetMyCompanyCreateRequestsQuery : IQuery<List<CompanyCreateRequestSummaryDto>>;

internal sealed class GetMyCompanyCreateRequestsQueryHandler : IQueryHandler<GetMyCompanyCreateRequestsQuery, List<CompanyCreateRequestSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetMyCompanyCreateRequestsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<List<CompanyCreateRequestSummaryDto>>> Handle(GetMyCompanyCreateRequestsQuery request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<List<CompanyCreateRequestSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var rawList = await _dbContext.CompanyCreateRequests
            .AsNoTracking()
            .Where(r => r.ProfileId == currentProfileId.Value)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new
            {
                r.Id,
                r.ProfileId,
                SubmitterName = r.Profile.FullName,
                SubmitterAvatarObjectKey = r.Profile.ProfilePictureObjectKey,
                r.Name,
                r.Industry,
                r.WebsiteUrl,
                r.LogoObjectKey,
                r.Status,
                r.CreatedAt,
                r.ReviewedAt
            })
            .ToListAsync(cancellationToken);

        var items = rawList.Select(r => new CompanyCreateRequestSummaryDto(
            Id: r.Id,
            ProfileId: r.ProfileId,
            SubmitterName: r.SubmitterName,
            SubmitterAvatarUrl: !string.IsNullOrWhiteSpace(r.SubmitterAvatarObjectKey)
                ? _fileStorageService.GetFilePublicUrl(r.SubmitterAvatarObjectKey)
                : null,
            Name: r.Name,
            Industry: r.Industry,
            WebsiteUrl: r.WebsiteUrl,
            LogoUrl: !string.IsNullOrWhiteSpace(r.LogoObjectKey)
                ? _fileStorageService.GetFilePublicUrl(r.LogoObjectKey)
                : null,
            Status: r.Status,
            CreatedAt: r.CreatedAt,
            ReviewedAt: r.ReviewedAt
        )).ToList();

        return Result<List<CompanyCreateRequestSummaryDto>>.Success(items, OperationStatusCode.Success);
    }
}

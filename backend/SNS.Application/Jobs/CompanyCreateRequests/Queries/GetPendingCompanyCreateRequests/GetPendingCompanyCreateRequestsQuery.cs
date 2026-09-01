using Microsoft.EntityFrameworkCore;
using SNS.Application.Jobs.CompanyCreateRequests.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Jobs.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Jobs.CompanyCreateRequests.Queries.GetPendingCompanyCreateRequests;

public sealed record GetPendingCompanyCreateRequestsQuery(
    int PageSize = 10,
    int CurrentPage = 1
) : IQuery<Paged<CompanyCreateRequestSummaryDto>>;

internal sealed class GetPendingCompanyCreateRequestsQueryHandler : IQueryHandler<GetPendingCompanyCreateRequestsQuery, Paged<CompanyCreateRequestSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetPendingCompanyCreateRequestsQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<CompanyCreateRequestSummaryDto>>> Handle(GetPendingCompanyCreateRequestsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.CompanyCreateRequests
            .AsNoTracking()
            .Where(r => r.Status == CompanyCreateRequestStatus.Pending);

        var totalCount = await query.CountAsync(cancellationToken);

        var rawList = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
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

        return Result<Paged<CompanyCreateRequestSummaryDto>>.Success(new Paged<CompanyCreateRequestSummaryDto>(
            items: items,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage), OperationStatusCode.Success);
    }
}

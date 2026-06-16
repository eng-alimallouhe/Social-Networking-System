using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Common;
using SNS.Application.Identity.ArchiveManagement.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Identity.ArchiveManagement.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.ArchiveManagement.Qureies.GetUserIdentityArchive;

public sealed class GetUserIdentityArchiveQueryHandler
    : IQueryHandler<GetUserIdentityArchiveQuery, Paged<UserIdentityArchiveSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGeneratorService _generatorService;

    public GetUserIdentityArchiveQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IGeneratorService generatorService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _generatorService = generatorService;
    }

    public async Task<Result<Paged<UserIdentityArchiveSummaryDto>>> Handle(
        GetUserIdentityArchiveQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var currentUserRole = _currentUserService.RoleType;

        if (currentUserId == null || currentUserId == Guid.Empty || string.IsNullOrWhiteSpace(currentUserRole))
        {
            return Result<Paged<UserIdentityArchiveSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (!currentUserRole.Contains("admin", StringComparison.OrdinalIgnoreCase) && currentUserId != request.TargetUserId)
        {
            return Result<Paged<UserIdentityArchiveSummaryDto>>.Failure(SecurityStatusCodes.AccessDenied);
        }

        var totalCount = await _dbContext.IdentityArchives
            .Where(a => a.UserId == request.TargetUserId)
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            var emptyList = new Paged<UserIdentityArchiveSummaryDto>(new List<UserIdentityArchiveSummaryDto>(), 0, request.CurrentPage, request.PageSize);
            return Result<Paged<UserIdentityArchiveSummaryDto>>.Success(emptyList, OperationStatusCode.Success);
        }

        var rawArchives = await _dbContext.IdentityArchives
            .Where(a => a.UserId == request.TargetUserId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new
            {
                a.Id,
                a.OldUserIdentifier,
                a.NewUserIdentifier,
                a.Type,
                a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var processedItems = rawArchives.Select(archive =>
        {
            string maskedOld = archive.Type == IdentityType.Email
                ? _generatorService.GenerateEmailMask(archive.OldUserIdentifier)
                : archive.OldUserIdentifier;

            string maskedNew = archive.Type == IdentityType.Email
                ? _generatorService.GenerateEmailMask(archive.NewUserIdentifier)
                : archive.NewUserIdentifier;

            return new UserIdentityArchiveSummaryDto(
                archive.Id,
                maskedOld,
                maskedNew,
                archive.Type,
                archive.CreatedAt);
        }).ToList();

        var paginatedResult = new Paged<UserIdentityArchiveSummaryDto>(
            items: processedItems,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage);

        return Result<Paged<UserIdentityArchiveSummaryDto>>.Success(paginatedResult, OperationStatusCode.Success);
    }
}
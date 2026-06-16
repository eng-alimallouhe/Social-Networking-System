using SNS.Application.Identity.ArchiveManagement.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;


namespace SNS.Application.Identity.ArchiveManagement.Qureies.GetUserPasswordArchive;

public sealed class GetUserPasswordArchiveQueryHandler
    : IQueryHandler<GetUserPasswordArchiveQuery, Paged<UserPasswordArchiveSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetUserPasswordArchiveQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Paged<UserPasswordArchiveSummaryDto>>> Handle(
        GetUserPasswordArchiveQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var currentUserRole = _currentUserService.RoleType;

        if (currentUserId == null || currentUserId == Guid.Empty || string.IsNullOrWhiteSpace(currentUserRole))
        {
            return Result<Paged<UserPasswordArchiveSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (!currentUserRole.Contains("admin", StringComparison.OrdinalIgnoreCase) && currentUserId != request.TargetUserId)
        {
            return Result<Paged<UserPasswordArchiveSummaryDto>>.Failure(SecurityStatusCodes.AccessDenied);
        }

        var totalCount = await _dbContext.PasswordArchives
            .Where(p => p.UserId == request.TargetUserId)
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            var emptyList = new Paged<UserPasswordArchiveSummaryDto>(new List<UserPasswordArchiveSummaryDto>(), 0, request.CurrentPage, request.PageSize);
            return Result<Paged<UserPasswordArchiveSummaryDto>>.Success(emptyList, OperationStatusCode.Success);
        }

        var processedItems = await _dbContext.PasswordArchives
            .Where(p => p.UserId == request.TargetUserId)
            .OrderByDescending(p => p.CreatedAt) 
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new UserPasswordArchiveSummaryDto(
                p.Id,
                p.CreatedAt)) 
            .ToListAsync(cancellationToken);

        var paginatedResult = new Paged<UserPasswordArchiveSummaryDto>(
            items: processedItems,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage);

        return Result<Paged<UserPasswordArchiveSummaryDto>>.Success(paginatedResult, OperationStatusCode.Success);
    }
}
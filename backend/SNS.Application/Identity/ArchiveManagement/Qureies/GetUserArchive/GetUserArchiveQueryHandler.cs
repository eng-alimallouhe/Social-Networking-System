using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.ArchiveManagement.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SNS.Application.Identity.ArchiveManagement.Qureies.GetUserArchive;

public sealed class GetUserArchiveQueryHandler
    : IQueryHandler<GetUserArchiveQuery, Paged<UserArchiveSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetUserArchiveQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Paged<UserArchiveSummaryDto>>> Handle(
        GetUserArchiveQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var currentUserRole = _currentUserService.RoleType;

        
        if (currentUserId == null || currentUserId == Guid.Empty || string.IsNullOrWhiteSpace(currentUserRole))
        {
            return Result<Paged<UserArchiveSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (!currentUserRole.Contains("admin", StringComparison.OrdinalIgnoreCase) && currentUserId != request.TargetUserId)
        {
            return Result<Paged<UserArchiveSummaryDto>>.Failure(SecurityStatusCodes.AccessDenied);
        }

        var totalCount = await _dbContext.UserArchives
            .Where(a => a.TargetId == request.TargetUserId)
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            var emptyList = new Paged<UserArchiveSummaryDto>(new List<UserArchiveSummaryDto>(), 0, request.CurrentPage, request.PageSize);
            return Result<Paged<UserArchiveSummaryDto>>.Success(emptyList, OperationStatusCode.Success);
        }

        var serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        serializerOptions.Converters.Add(new JsonStringEnumConverter());

        var rawArchives = await _dbContext.UserArchives
            .Where(a => a.TargetId == request.TargetUserId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new
            {
                a.Id,
                a.Type,
                a.Reason,
                a.PerformedById,
                a.Parameters, 
                a.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var processedItems = rawArchives.Select(archive =>
        {
            string adminName = archive.PerformedById == null
                ? "System / Self"
                : _dbContext.Users.Where(u => u.Id == archive.PerformedById).Select(u => u.UserName).FirstOrDefault() ?? "Unknown Admin";

            Dictionary<ReplacementKey, string>? parsedParameters = null;
            if (!string.IsNullOrWhiteSpace(archive.Parameters))
            {
                try
                {
                    parsedParameters = JsonSerializer.Deserialize<Dictionary<ReplacementKey, string>>(archive.Parameters, serializerOptions);
                }
                catch (JsonException)
                {
                    parsedParameters = new Dictionary<ReplacementKey, string>();
                }
            }

            return new UserArchiveSummaryDto(
                archive.Id,
                archive.Type,
                archive.Reason ?? "No reason provided",
                archive.PerformedById,
                adminName,
                parsedParameters, 
                archive.CreatedAt);
        }).ToList();

        var paginatedResult = new Paged<UserArchiveSummaryDto>(
            items: processedItems,
            count: totalCount,
            pageSize: request.PageSize,
            currentPage: request.CurrentPage);

        return Result<Paged<UserArchiveSummaryDto>>.Success(paginatedResult, OperationStatusCode.Success);
    }
}
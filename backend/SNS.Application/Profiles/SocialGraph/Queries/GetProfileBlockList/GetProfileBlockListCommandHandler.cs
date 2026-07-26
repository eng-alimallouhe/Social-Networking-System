using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.SocialGraph.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Profiles.SocialGraph.Queries.GetProfileBlockList;

public sealed record BlockedProfileDto(
    Guid ProfileId, 
    string FullName,
    string? ProfilePictureUrl,
    string? Specialization,
    DateTime BlockedAt
);

public sealed record GetProfileBlockListQuery(
    string? SearchTerm,
    int CurrentPage = 1,
    int PageSize = 15
) : IQuery<Paged<BlockedProfileDto>>;

internal sealed class GetProfileBlockListQueryHandler
    : IQueryHandler<GetProfileBlockListQuery, Paged<BlockedProfileDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetProfileBlockListQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)

    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<BlockedProfileDto>>> Handle(GetProfileBlockListQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (!profileId.HasValue)
        {
            return Result<Paged<BlockedProfileDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var query = _dbContext
            .Blocks
            .Where(pv => pv.BlockerId == profileId.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var search = request.SearchTerm.Trim();

            query = query.Where(f =>
                f.Blocked.FullName.Contains(search) ||
                (f.Blocked.Specialization != null &&
                 f.Blocked.Specialization.Contains(search)));
        }

        var profileBlocksCount = await query.CountAsync(cancellationToken);

        var blockedProfiles = await query
            .OrderByDescending(pv => pv.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(pv => new
            {
                ProfileId = pv.BlockedId,
                FullName = pv.Blocked.FullName,
                ProfilePictureObjectKey = pv.Blocked.ProfilePictureObjectKey,
                Specialization = pv.Blocked.Specialization,
                BlockDate = pv.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var blockedProfilesResult = blockedProfiles.Select(pv => new BlockedProfileDto
        (
            ProfileId: pv.ProfileId,
            FullName: pv.FullName,
            ProfilePictureUrl: pv.ProfilePictureObjectKey != null ? _fileStorageService.GetFilePublicUrl(pv.ProfilePictureObjectKey) : null,
            Specialization: pv.Specialization,
            BlockedAt: pv.BlockDate
        )).ToList();

        return Result<Paged<BlockedProfileDto>>.Success(new Paged<BlockedProfileDto>(
            items: blockedProfilesResult,
            currentPage: request.CurrentPage,
            pageSize: request.PageSize,
            count: profileBlocksCount),
            ResourceStatusCode.Found);
    }
}

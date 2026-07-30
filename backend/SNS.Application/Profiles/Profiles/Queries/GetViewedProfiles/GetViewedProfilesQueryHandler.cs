using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Queries.GetViewedProfiles;

/// <summary>
/// Handles the execution of <see cref="GetViewedProfilesQuery"/> to retrieve viewed profiles history.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Resolves current user profile ID.
/// 2. Queries profile view records where the current user is the viewer.
/// 3. Applies descending ordering by view timestamp, pagination skipping, and page size limits.
/// 4. Maps target profile picture keys to public storage URLs and returns <see cref="Paged{ProfileViewDto}"/>.
/// </remarks>
internal sealed class GetViewedProfilesQueryHandler : IQueryHandler<GetViewedProfilesQuery, Paged<ProfileViewDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetViewedProfilesQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }
    public async Task<Result<Paged<ProfileViewDto>>> Handle(GetViewedProfilesQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (profileId == null)
        {
            return Result<Paged<ProfileViewDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var query = _dbContext
            .ProfileViews
            .Where(pv => pv.ViewerId == profileId.Value);

        var profileViewsCount = await query.CountAsync(cancellationToken);

        var viewedProfiles = await query
            .OrderByDescending(pv => pv.ViewedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(pv => new
            {
                ProfileId = pv.ViewedId,
                FullName = pv.Viewed.FullName,
                ProfilePictureObjectKey = pv.Viewed.ProfilePictureObjectKey,
                Specialization = pv.Viewed.Specialization,
                ViewedAt = pv.ViewedAt
            })
            .ToListAsync(cancellationToken);
        
        var viewedProfileResult = viewedProfiles.Select(pv => new ProfileViewDto
        (
            ProfileId: pv.ProfileId,
            FullName: pv.FullName,
            ProfilePictureUrl: pv.ProfilePictureObjectKey != null ? _fileStorageService.GetFilePublicUrl(pv.ProfilePictureObjectKey) : null,
            Specialization: pv.Specialization,
            ViewedAt: pv.ViewedAt
        )).ToList();

        return Result<Paged<ProfileViewDto>>.Success(new Paged<ProfileViewDto>(
            items: viewedProfileResult, 
            currentPage: request.CurrentPage, 
            pageSize: request.PageSize,
            count: profileViewsCount), 
            ResourceStatusCode.Found);
    }
}
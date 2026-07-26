using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;


namespace SNS.Application.Profiles.Profiles.Queries.GetProfileViewers;

internal sealed class GetProfileViewersQueryHandler : IQueryHandler<GetProfileViewersQuery, Paged<ProfileViewDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetProfileViewersQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }
    public async Task<Result<Paged<ProfileViewDto>>> Handle(GetProfileViewersQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (profileId == null)
        {
            return Result<Paged<ProfileViewDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var query = _dbContext
            .ProfileViews
            .Where(pv => pv.ViewedId == profileId.Value);

        var profileViewsCount = await query.CountAsync(cancellationToken);

        var viewedProfiles = await query
            .OrderByDescending(pv => pv.ViewedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(pv => new
            {
                ProfileId = pv.ViewerId,
                FullName = pv.Viewed.FullName,
                ProfilePictureObjectKey = pv.Viewer.ProfilePictureObjectKey,
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
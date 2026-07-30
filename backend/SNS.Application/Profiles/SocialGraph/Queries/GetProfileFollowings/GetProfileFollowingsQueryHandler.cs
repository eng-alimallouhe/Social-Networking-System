using Microsoft.EntityFrameworkCore;
using SNS.Application.Profiles.SocialGraph.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Profiles.SocialGraph.Queries.GetProfileFollowings;

/// <summary>
/// Handles the execution of <see cref="GetProfileFollowingsQuery"/> to retrieve profile followings.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries follow records where the specified profile is the follower (<c>FollowerId</c>).
/// 2. Applies search term filtering on followed full name and specialization if specified.
/// 3. Sorts by follow date descending and applies pagination (<c>Skip</c>/<c>Take</c>).
/// 4. Maps profile picture keys to public storage URLs and returns <see cref="Paged{ProfileFollowDto}"/>.
/// </remarks>
internal sealed class GetProfileFollowingsQueryHandler
    : IQueryHandler<GetProfileFollowingsQuery, Paged<ProfileFollowDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetProfileFollowingsQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<Paged<ProfileFollowDto>>> Handle(GetProfileFollowingsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext
            .Follows
            .Where(pv => pv.FollowerId == request.ProfileId);

        if (request.SearchTerm != null)
        {
            var search = request.SearchTerm.Trim();

            query = query.Where(f =>
                f.Following.FullName.Contains(search) ||
                (f.Following.Specialization != null &&
                 f.Following.Specialization.Contains(search)));
        }

        var profileViewsCount = await query.CountAsync(cancellationToken);

        var followers = await query
            .OrderByDescending(pv => pv.CreatedAt)
            .Skip((request.CurrentPage - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(pv => new
            {
                ProfileId = pv.FollowingId,
                FullName = pv.Following.FullName,
                ProfilePictureObjectKey = pv.Following.ProfilePictureObjectKey,
                Specialization = pv.Following.Specialization,
                FollowDate = pv.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var followersResult = followers.Select(pv => new ProfileFollowDto
        (
            ProfileId: pv.ProfileId,
            FullName: pv.FullName,
            ProfilePictureUrl: pv.ProfilePictureObjectKey != null ? _fileStorageService.GetFilePublicUrl(pv.ProfilePictureObjectKey) : null,
            Specialization: pv.Specialization,
            FollowDate: pv.FollowDate
        )).ToList();

        return Result<Paged<ProfileFollowDto>>.Success(new Paged<ProfileFollowDto>(
            items: followersResult,
            currentPage: request.CurrentPage,
            pageSize: request.PageSize,
            count: profileViewsCount),
            ResourceStatusCode.Found);
    }
}
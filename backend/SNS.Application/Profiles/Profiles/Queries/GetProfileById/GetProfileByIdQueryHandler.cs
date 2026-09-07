using Microsoft.EntityFrameworkCore;
using SNS.Application.Education.Shared.DTOs;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileById;

/// <summary>
/// Handles the execution of <see cref="GetProfileByIdQuery"/> to retrieve comprehensive profile details.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Verifies viewer identity and checks for block relationship restrictions.
/// 2. Executes a read-only query (<c>AsNoTracking</c>) projecting full profile metrics, skills, academic records, social links, follower/following counts, and viewer relationship flags.
/// 3. Materializes the query, then resolves the public storage URL for the profile picture using <see cref="IFileStorageService.GetFilePublicUrl"/>.
/// </remarks>
public sealed record GetProfileByIdQueryHandler : IQueryHandler<GetProfileByIdQuery, ProfileDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetProfileByIdQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<ProfileDetailsDto>> Handle(GetProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profileId = request.profileId;
        var viewerId = _currentUserService.ProfileId;

        if (viewerId == null)
        {
            return Result<ProfileDetailsDto>.Failure(OperationStatusCode.AuthenticationRequired);
        }

        bool isBlocked = await _dbContext.Blocks
            .AnyAsync(b => b.BlockerId == profileId && b.BlockedId == viewerId, cancellationToken);

        if (isBlocked)
        {
            return Result<ProfileDetailsDto>.Failure(UserStatusCodes.NotFound);
        }

        var profileData = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => p.Id == profileId)
            .Select(p => new
            {
                p.Id,
                p.FullName,
                p.Bio,
                p.ProfilePictureObjectKey,
                p.Specialization,
                FollowersCount = p.Followers.Count(),
                FollowingsCount = p.Followings.Count(),
                ViewsCount = p.Vieweds.Count(),
                Skills = p.ProfileSkills.Select(ps => new ProfileSkillDto(
                    ps.Id,
                    ps.SkillId,
                    ps.Skill.Name,
                    ps.Level
                )).ToList(),
                AcademicRecords = p.AcademicRecords.Select(ar => new AcademicRecordSummaryDto(
                    ar.University!.Name,
                    ar.FieldOfStudy
                )).ToList(),
                p.Location,
                p.GitHubUrl,
                p.LinkedInUrl,
                p.XUrl,
                p.FacebookUrl,
                p.Website,
                IsFollowedByViewer = p.Followers.Any(f => f.FollowerId == viewerId),
                IsBlockedByViewer = _dbContext.Blocks.Any(b => b.BlockerId == viewerId && b.BlockedId == profileId),
                IsViewerOwner = profileId == viewerId,
                IsBlockingViewer = false
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (profileData is null)
        {
            return Result<ProfileDetailsDto>.Failure(UserStatusCodes.NotFound);
        }

        string? profilePictureUrl = null;
        if (!string.IsNullOrWhiteSpace(profileData.ProfilePictureObjectKey))
        {
            profilePictureUrl = await _fileStorageService.GetTemporaryUrlAsync(
                profileData.ProfilePictureObjectKey,
                TimeSpan.FromHours(1));
        }

        var profile = new ProfileDetailsDto(
            profileData.Id,
            profileData.FullName,
            profileData.Bio,
            profilePictureUrl,
            profileData.Specialization,
            profileData.FollowersCount,
            profileData.FollowingsCount,
            profileData.ViewsCount,
            profileData.Skills,
            profileData.AcademicRecords,
            profileData.Location,
            profileData.GitHubUrl,
            profileData.LinkedInUrl,
            profileData.XUrl,
            profileData.FacebookUrl,
            profileData.Website,
            profileData.IsFollowedByViewer,
            profileData.IsBlockedByViewer,
            profileData.IsViewerOwner,
            profileData.IsBlockingViewer
        );

        return Result<ProfileDetailsDto>.Success(profile, OperationStatusCode.Success);
    }
}

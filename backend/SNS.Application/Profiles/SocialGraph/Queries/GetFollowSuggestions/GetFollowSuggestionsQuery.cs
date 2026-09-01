using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.SocialGraph.Queries.GetFollowSuggestions;

/// <summary>
/// Represents a query to retrieve follow suggestions for the authenticated profile.
/// </summary>
public sealed record GetFollowSuggestionsQuery : IQuery<List<ProfileSummaryDto>>;

/// <summary>
/// Handles the execution of <see cref="GetFollowSuggestionsQuery"/> to retrieve follow suggestions based on social connections, shared skills, and popularity.
/// </summary>
/// <remarks>
/// Query resolution flow:
/// 1. Resolves authenticated profile ID from <see cref="ICurrentUserService"/>.
/// 2. Filters out the user's own profile, already-followed profiles, and bidirectional blocked relationships.
/// 3. Ranks candidates deterministically by:
///    - Mutual followers count (connections of followed profiles who follow candidate).
///    - Shared skills count with the current user.
///    - Total follower count.
///    - CreatedAt timestamp descending.
/// 4. Limits results to 10 profiles and maps storage keys to public URLs.
/// </remarks>
internal sealed class GetFollowSuggestionsQueryHandler
    : IQueryHandler<GetFollowSuggestionsQuery, List<ProfileSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetFollowSuggestionsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<List<ProfileSummaryDto>>> Handle(
        GetFollowSuggestionsQuery request,
        CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result<List<ProfileSummaryDto>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var currentProfileId = profileId.Value;

        var followedUserIds = _dbContext.Follows
            .Where(f => f.FollowerId == currentProfileId)
            .Select(f => f.FollowingId);

        var userSkillIds = _dbContext.ProfileSkills
            .Where(ps => ps.ProfileId == currentProfileId)
            .Select(ps => ps.SkillId);

        var blockedUserIds = _dbContext.Blocks
            .Where(b => b.BlockerId == currentProfileId)
            .Select(b => b.BlockedId);

        var blockerUserIds = _dbContext.Blocks
            .Where(b => b.BlockedId == currentProfileId)
            .Select(b => b.BlockerId);

        var query = _dbContext.Profiles
            .Where(p => p.IsActive)
            .Where(p => p.Id != currentProfileId)
            .Where(p => !followedUserIds.Contains(p.Id))
            .Where(p => !blockedUserIds.Contains(p.Id))
            .Where(p => !blockerUserIds.Contains(p.Id));

        var suggestedProfiles = await query
            .OrderByDescending(p => p.Followers.Count(f => followedUserIds.Contains(f.FollowerId)))
            .ThenByDescending(p => p.ProfileSkills.Count(ps => userSkillIds.Contains(ps.SkillId)))
            .ThenByDescending(p => p.Followers.Count())
            .ThenByDescending(p => p.CreatedAt)
            .Take(10)
            .Select(p => new
            {
                p.Id,
                p.FullName,
                p.Specialization,
                p.Bio,
                p.ProfilePictureObjectKey,
                FollowersCount = p.Followers.Count(),
                FollowingCount = p.Followings.Count(),
                Skills = p.ProfileSkills.Select(ps => ps.Skill.Name).ToList(),
                p.CreatedAt
            })
            .ToListAsync(cancellationToken);

        var results = suggestedProfiles.Select(p => new ProfileSummaryDto(
            Id: p.Id,
            FullName: p.FullName,
            Specialization: p.Specialization,
            Bio: p.Bio,
            ProfilePictureUrl: p.ProfilePictureObjectKey != null ? _fileStorageService.GetFilePublicUrl(p.ProfilePictureObjectKey) : null,
            FollowersCount: p.FollowersCount,
            FollowingCount: p.FollowingCount,
            Skills: p.Skills,
            CreatedAt: p.CreatedAt,
            IsFollowedByCurrentUser: false,
            IsBlockedByCurrentUser: false
        )).ToList();

        return Result<List<ProfileSummaryDto>>.Success(results, ResourceStatusCode.Found);
    }
}

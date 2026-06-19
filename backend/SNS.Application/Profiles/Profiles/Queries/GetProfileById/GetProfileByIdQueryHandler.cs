using Microsoft.EntityFrameworkCore;
using SNS.Application.Education.Shared.DTOs;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileById;

public sealed record GetProfileByIdQueryHandler : IQueryHandler<GetProfileByIdQuery, ProfileDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetProfileByIdQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
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
        var profile = await _dbContext.Profiles
    .AsNoTracking() // 🚀 حتمي للأداء لتقليل استهلاك الـ CPU والـ Memory
    .Where(p => p.Id == profileId)
    .Select(p => new ProfileDetailsDto
    (
        p.Id,
        p.FullName,
        p.Bio,
        p.ProfilePictureUrl,
        p.Specialization,
        p.Followers.Count(),
        p.Followings.Count(),
        p.Vieweds.Count(),
        p.ProfileSkills.Select(ps => new ProfileSkillDto(
            ps.Id,
            ps.SkillId,
            ps.Skill.Name,
            ps.Level
        )).ToList(),
        p.AcademicRecords.Select(ar => new AcademicRecordSummaryDto(
            ar.University!.Name,
            ar.FieldOfStudy
        )).ToList(),
        p.Location,
        p.GitHubUrl,
        p.LinkedInUrl,
        p.XUrl,
        p.FacebookUrl,
        p.Website,
        p.Followers.Any(f => f.FollowerId == viewerId),
        _dbContext.Blocks.Any(b => b.BlockerId == viewerId && b.BlockedId == profileId),
        profileId == viewerId,
        false
    ))
    .FirstOrDefaultAsync(cancellationToken);

        if (profile is null)
        {
            return Result<ProfileDetailsDto>.Failure(UserStatusCodes.NotFound);
        }

        return Result<ProfileDetailsDto>.Success(profile, OperationStatusCode.Success);
    }
}

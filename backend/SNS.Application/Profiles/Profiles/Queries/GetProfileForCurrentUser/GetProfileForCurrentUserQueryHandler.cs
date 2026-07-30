using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Settings;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileForCurrentUser;

/// <summary>
/// Handles the execution of <see cref="GetProfileForCurrentUserQuery"/> to retrieve the authenticated user's base profile summary.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Resolves current authenticated user ID.
/// 2. Projects profile fields (ID, full name, specialization, profile picture key, reputation).
/// 3. Applies fallback settings from <see cref="ProfileSettings"/> if specialization or avatar key is missing.
/// </remarks>
public sealed class GetProfileForCurrentUserQueryHandler : IRequestHandler<GetProfileForCurrentUserQuery, Result<ProfileBaseDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ProfileSettings _profileSettings;

    public GetProfileForCurrentUserQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IOptions<ProfileSettings> options)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _profileSettings = options.Value;
    }
    
    public async Task<Result<ProfileBaseDto>> Handle(GetProfileForCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return Result<ProfileBaseDto>.Failure(OperationStatusCode.AuthenticationRequired);            
        }

        var profile = await _dbContext.Profiles
            .Where(p => p.UserId == userId)
            .Select(p => new ProfileBaseDto(
                Id: p.Id,
                FullName: p.FullName,
                Specialization: p.Specialization ?? _profileSettings.DefaultSpecialization,
                ProfilePictureUrl: p.ProfilePictureObjectKey ?? _profileSettings.DefaultProfilePictureUrl,
                Reputation: p.Reputation
            )).FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
        {
            return Result<ProfileBaseDto>.Failure(UserStatusCodes.ProfileNotCompleted);
        }

        return Result<ProfileBaseDto>.Success(profile, OperationStatusCode.Success);
    }
}

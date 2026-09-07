using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.Settings;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileForCurrentUser;

/// <summary>
/// Handles the execution of <see cref="GetProfileForCurrentUserQuery"/> to retrieve the authenticated user's base profile summary.
/// </summary>
public sealed class GetProfileForCurrentUserQueryHandler : IRequestHandler<GetProfileForCurrentUserQuery, Result<ProfileBaseDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ProfileSettings _profileSettings;
    private readonly IFileStorageService _fileStorageService;

    public GetProfileForCurrentUserQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IOptions<ProfileSettings> options,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _profileSettings = options.Value;
        _fileStorageService = fileStorageService;
    }
    
    public async Task<Result<ProfileBaseDto>> Handle(GetProfileForCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return Result<ProfileBaseDto>.Failure(OperationStatusCode.AuthenticationRequired);            
        }

        var profileData = await _dbContext.Profiles
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.Id,
                p.FullName,
                Specialization = p.Specialization ?? _profileSettings.DefaultSpecialization,
                p.ProfilePictureObjectKey,
                p.Reputation
            }).FirstOrDefaultAsync(cancellationToken);

        if (profileData == null)
        {
            return Result<ProfileBaseDto>.Failure(UserStatusCodes.ProfileNotCompleted);
        }

        string? profilePictureUrl = null;
        if (!string.IsNullOrWhiteSpace(profileData.ProfilePictureObjectKey))
        {
            profilePictureUrl = await _fileStorageService.GetTemporaryUrlAsync(
                profileData.ProfilePictureObjectKey,
                TimeSpan.FromHours(1));
        }
        else if (!string.IsNullOrWhiteSpace(_profileSettings.DefaultProfilePictureUrl))
        {
            profilePictureUrl = _profileSettings.DefaultProfilePictureUrl;
        }

        var profile = new ProfileBaseDto(
            profileData.Id,
            profileData.FullName,
            profileData.Specialization,
            profilePictureUrl ?? string.Empty,
            profileData.Reputation
        );

        return Result<ProfileBaseDto>.Success(profile, OperationStatusCode.Success);
    }
}

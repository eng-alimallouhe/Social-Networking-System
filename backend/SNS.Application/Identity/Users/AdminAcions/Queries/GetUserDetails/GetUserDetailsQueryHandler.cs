using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserSessions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Settings;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.AdminAcions.Queries.GetUserDetails;

public sealed class GetUserDetailsQueryHandler : IQueryHandler<GetUserDetailsQuery, UserDetailsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    private readonly ProfileSettings _profileSettings;

    public GetUserDetailsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IOptions<ProfileSettings> options)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _profileSettings = options.Value;
    }

    public async Task<Result<UserDetailsDto>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var currentUserRole = _currentUserService.RoleType;

        if (currentUserId == null || currentUserId == Guid.Empty || string.IsNullOrWhiteSpace(currentUserRole))
        {
            return Result<UserDetailsDto>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (!currentUserRole.Contains("admin", StringComparison.OrdinalIgnoreCase))
        {
            return Result<UserDetailsDto>.Failure(SecurityStatusCodes.AccessDenied);
        }

        var defaultProfileSpecialization = _profileSettings.DefaultSpecialization;
        var defaultProfilePictureUrl = _profileSettings.DefaultProfilePictureUrl;

        var user = await _dbContext
            .Users
            .Where(u => u.Id == request.TargetUserId)
            .Select(u => new UserDetailsDto(
                u.Id,
                u.UserName,
                u.Email,
                u.Status,
                u.IsVerified,
                u.FailedLoginAttempts,
                u.PreferredLanguage,
                u.CreatedAt,
                u.UpdatedAt,
                u.LastLogIn,
                u.LastPasswordChange,
                u.SuspendedUntil,
                u.SuspensionReason,
                u.DeactivatedAt,
                new ProfileBaseDto(
                    u.UserProfile.Id,
                    u.UserProfile.FullName,
                    u.UserProfile.Specialization ?? defaultProfileSpecialization,
                    u.UserProfile.ProfilePictureObjectKey ?? defaultProfilePictureUrl,
                    u.UserProfile.Reputation),
                new UserRoleDetailsDto(
                    u.RoleId,
                    u.Role.Type),
                new UserMetricsDto(
                    u.Sessions.Count(s => s.LogoutAt == null),
                    u.Devices.Count,
                    u.Passkeys.Count,
                    u.IdentityArchives.Count,
                    u.PasswordArchives.Count),
                u.Sessions
                .Where(s => s.LogoutAt == null)
                .Select(s => new SessionSummaryDto(
                    s.UserId,
                    s.Id,
                    s.Device.FriendlyName,
                    s.LoginAt,
                    s.LastSeenAt,
                    s.LogoutAt,
                    s.Country,
                    s.City,
                    s.DurationMinutes,
                    s.IsRevoked,
                    s.RevokedReason))
                .ToList(),
                u.Sessions
                .OrderBy(s => s.LoginAt)
                .Select(s => new SessionSummaryDto(
                    s.UserId,
                    s.Id,
                    s.Device.FriendlyName,
                    s.LoginAt,
                    s.LastSeenAt,
                    s.LogoutAt,
                    s.Country,
                    s.City,
                    s.DurationMinutes,
                    s.IsRevoked,
                    s.RevokedReason))
                .ToList()
            )).FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return Result<UserDetailsDto>.Failure(UserStatusCodes.NotFound);
        }

        return Result<UserDetailsDto>.Success(user, OperationStatusCode.Success);
    }
}
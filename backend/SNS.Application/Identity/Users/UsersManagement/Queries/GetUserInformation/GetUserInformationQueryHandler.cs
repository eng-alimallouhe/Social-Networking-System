using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserInformation;

/// <summary>
/// Handles the execution of <see cref="GetUserInformationQuery"/> to retrieve current user account details.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Verifies authentication of the requesting user.
/// 2. Performs a non-tracking query (<c>AsNoTracking</c>) projecting user details, role, location, and latest session location.
/// 3. Checks for existing data export requests to include in the <see cref="UserInformationResult"/>.
/// </remarks>
public sealed class GetUserInformationQueryHandler : IQueryHandler<GetUserInformationQuery, UserInformationResult>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetUserInformationQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserInformationResult>> Handle(GetUserInformationQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null || userId == Guid.Empty)
        {
            return Result<UserInformationResult>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        // 1️⃣ استغلال الـ Navigation Properties لجلب بيانات المستخدم، البروفايل، وآخر جلسة بضربة واحدة ناصعة 🏎️
        var userData = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.UserName,
                RoleName = u.Role.Type,
                u.Email,
                u.PreferredLanguage,
                u.LastPasswordChange,
                Location = u.UserProfile.Location,
                LastActiveLocation = u.Sessions
                    .Where(s => !s.IsRevoked)
                    .OrderByDescending(s => s.LoginAt)
                    .Select(s => $"{s.City}, {s.Country}")
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userData == null)
        {
            return Result<UserInformationResult>.Failure(UserStatusCodes.NotFound);
        }

        var hasActiveDownloadRequest = await _dbContext.ExportDataRequests
            .AnyAsync(r => r.UserId == userId, cancellationToken);

        var result = new UserInformationResult(
            UserName: userData.UserName,
            RoleName: userData.RoleName.ToString(),
            Email: userData.Email,
            PreferredLanguage: userData.PreferredLanguage,
            LastPasswordChange: userData.LastPasswordChange,
            Location: userData.Location,
            LastActiveLocation: userData.LastActiveLocation,
            HasActiveDataDownloadRequest: hasActiveDownloadRequest
        );

        return Result<UserInformationResult>.Success(result, OperationStatusCode.Success);
    }
}
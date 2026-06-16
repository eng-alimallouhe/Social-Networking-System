using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Identity.Notifications.Queries.GetUserNotificationPreferences;


public sealed class GetUserNotificationPreferencesQueryHandler
    : IQueryHandler<GetUserNotificationPreferencesQuery, UserNotificationPreferencesDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetUserNotificationPreferencesQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserNotificationPreferencesDto>> Handle(
        GetUserNotificationPreferencesQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId == null || currentUserId == Guid.Empty)
        {
            return Result<UserNotificationPreferencesDto>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var preferencesDto = await _dbContext 
            .UserNotificationPreferences
            .Where(p => p.UserId == currentUserId)
            .Select(p => new UserNotificationPreferencesDto(
                p.NewFollower,
                p.PostLikes,
                p.PostComments,
                p.CommentReplies,
                p.Mentions,
                p.Messages,
                p.CommunityPosts,
                p.CommunityAnnouncements,
                p.ProjectInvitations,
                p.ProjectUpdates,
                p.ProblemSolutions,
                p.LoginAlerts,
                p.PasswordChanged,
                p.EnableEmailNotifications,
                p.EnableSmsNotifications,
                p.EnablePushNotifications,
                p.EnableInAppNotifications
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (preferencesDto == null)
        {
            var defaultPreferences = new UserNotificationPreferencesDto(
                false, false, false, false, false, false, false, false,
                false, false, false, false, false, false, false, false, false
            );
            return Result<UserNotificationPreferencesDto>.Success(defaultPreferences, OperationStatusCode.Success);
        }

        // 4️⃣ العودة بالنتيجة الفاخرة المنسقة لعلي 🎁
        return Result<UserNotificationPreferencesDto>.Success(preferencesDto, OperationStatusCode.Success);
    }
}
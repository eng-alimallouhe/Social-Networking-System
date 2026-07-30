using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.Notifications.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Notifications.Commands.UpdateNotificationPreferences;

/// <summary>
/// Handles the execution of <see cref="UpdateNotificationPreferencesCommand"/> to update user notification preferences.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user ID.
/// 2. Fetches existing preferences entity or creates a new record if one does not exist.
/// 3. Updates social, community, project, problem, security preferences, and channel delivery flags.
/// 4. Saves entity changes to database.
/// Side effects include creation or modification of user notification preferences and database persistence.
/// </remarks>
public sealed class UpdateNotificationPreferencesCommandHandler
    : ICommandHandler<UpdateNotificationPreferencesCommand>
{
    private readonly IRepository<UserNotificationPreferences> _preferencesRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateNotificationPreferencesCommandHandler(
        IRepository<UserNotificationPreferences> preferencesRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _preferencesRepo = preferencesRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        UpdateNotificationPreferencesCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId == null || currentUserId == Guid.Empty)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var preferences = await _preferencesRepo.GetSingleByExpressionAsync(
            p => p.UserId == currentUserId,
            cancellationToken);

        if (preferences == null)
        {
            preferences = UserNotificationPreferences.Create(currentUserId.Value);
            _preferencesRepo.Add(preferences);
        }

        preferences.UpdateSocialPreferences(
            request.NewFollower,
            request.PostLikes,
            request.PostComments,
            request.CommentReplies,
            request.Mentions,
            request.Messages);

        preferences.UpdateCommunityPreferences(
            request.CommunityPosts,
            request.CommunityAnnouncements);

        preferences.UpdateProjectPreferences(
            request.ProjectInvitations,
            request.ProjectUpdates);

        preferences.UpdateProblemPreferences(request.ProblemSolutions);

        preferences.UpdateSecurityPreferences(
            request.LoginAlerts,
            request.PasswordChanged);

        preferences.UpdateDeliveryChannels(
            request.EnableEmailNotifications,
            request.EnableSmsNotifications,
            request.EnablePushNotifications,
            request.EnableInAppNotifications);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}
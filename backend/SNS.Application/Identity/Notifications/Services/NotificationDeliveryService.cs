using SNS.Application.Identity.Notifications.Abstractions;
using SNS.Application.Identity.Notifications.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Identity.Notifications.Services;

public class NotificationDeliveryService
    : INotificationDeliveryService
{
    private readonly INotificationLocalizerService _notificationLocalizerService;
    private readonly IOnlineUserTracker _onlineUserTracker;
    private readonly IInAppNotificationService _inAppNotificationService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IPushNotificationService _pushNotificationService;

    public NotificationDeliveryService(
        INotificationLocalizerService notificationLocalizerService,
        IOnlineUserTracker onlineUserTracker,
        IInAppNotificationService notificationHubService,
        IApplicationDbContext dbContext,
        IPushNotificationService pushNotificationService)
    {
        _notificationLocalizerService = notificationLocalizerService;
        _onlineUserTracker = onlineUserTracker;
        _inAppNotificationService = notificationHubService;
        _dbContext = dbContext;
        _pushNotificationService = pushNotificationService;
    }

    public async Task SendAsync(NotificationDto notificationDto)
    {
        var userId = notificationDto.UserId;

        var notificationContent = _notificationLocalizerService.Localize(notificationDto.Type, notificationDto.Language, notificationDto.Arguments);

        if (!_onlineUserTracker.IsOnline(userId))
        {
            var pushTokens = await _dbContext
                .Devices
                .Where(d => d.UserId == notificationDto.UserId && d.PushTarget != null)
                .Select(d => d.PushTarget)
                .ToListAsync();

            await Task.WhenAll(pushTokens.Select(token =>
                _pushNotificationService.SendAsync(new PushNotificationDto(
                        Title: notificationContent.Title,
                        Body: notificationContent.Body,
                        ImageUrl: notificationDto.ActorPictureUrl,
                        PushTarget: token!,
                        RedirectUrl: notificationDto.RedirectUrl))));
        }
        else
        {
            await _inAppNotificationService.SendNotificationToUserAsync(new InAppNotificationDto(
                UserId: userId,
                Title: notificationContent.Title,
                Body: notificationContent.Body,
                TargetProfilePictureUrl: notificationDto.ActorPictureUrl));
        }
    }
}

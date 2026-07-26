using FirebaseAdmin.Messaging;
using SNS.Application.Identity.Notifications.Abstractions;
using SNS.Application.Identity.Notifications.Contracts;

namespace SNS.Infrastructure.Identity.Notifications.Services;

public sealed class PushNotificationService :
    IPushNotificationService
{
    public async Task SendAsync(PushNotificationDto notification)
    {
        await FirebaseMessaging.DefaultInstance.SendAsync(
            new Message
            {
                Fid = notification.PushTarget,

                Notification = new Notification
                {
                    Title = notification.Title,
                    Body = notification.Body,
                    ImageUrl = notification.ImageUrl
                },

                Data = new Dictionary<string, string>
                {
                    ["url"] = notification.RedirectUrl
                }
            });
    }
}
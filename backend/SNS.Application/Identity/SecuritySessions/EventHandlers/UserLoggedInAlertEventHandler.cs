using MediatR;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.SecuritySessions.Events;
using SNS.Domain.Identity.Shared.Enums;
namespace SNS.Application.Identity.SecuritySessions.EventHandlers;

public class UserLoggedInAlertEventHandler : INotificationHandler<DomainEventNotification<UserLoggedInEvent>>
{
    private readonly IUrlGeneratorService _urlGenerator;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly IAppLogger<UserLoggedInAlertEventHandler> _appLogger;

    public UserLoggedInAlertEventHandler(
        IUrlGeneratorService urlGenerator,
        IEmailSenderService emailSenderService,
        IEmailTemplateProvider emailTemplateProvider,
        IAppLogger<UserLoggedInAlertEventHandler> appLogger)

    {
        _urlGenerator = urlGenerator;
        _emailSenderService = emailSenderService;
        _emailTemplateProvider = emailTemplateProvider;
        _appLogger = appLogger;
    }


    public async Task Handle(DomainEventNotification<UserLoggedInEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var redirectUrl = _urlGenerator.GenerateSecurityEventUrl(domainEvent.UserId);

        var replacements = new List<MessageReplacement>()
        {
            new MessageReplacement (Key: ReplacementKey.IpAddress, Value: domainEvent.IpAddress),
            new MessageReplacement (Key: ReplacementKey.Device, Value: domainEvent.DeviceName),
            new MessageReplacement (Key: ReplacementKey.RedirectUrl,Value: redirectUrl)
        };

        var emailContent = await _emailTemplateProvider.ReadTemplate(
            domainEvent.UserLanguage,
            SendPurpose.LoginAlert,
            replacements);

        try
        {
            var email = _emailTemplateProvider.ReadTemplate(
                language: domainEvent.UserLanguage,
                purpose: SendPurpose.LoginAlert,
                replacements: replacements);


            var sendResult = await _emailSenderService.SendEmailAsync(
                toEmail: domainEvent.RecipientAddress,
                subject: emailContent.Subject,
                message: emailContent.Body);

            if (sendResult.IsFailure)
            {
                _appLogger.LogWarning(
                    "Failed to send login alert for user {UserId}. Reason: {Reason}, At Date: {Date}",
                    domainEvent.UserId, sendResult.StatusCode, DateTime.UtcNow);
            }
        }
        catch (Exception ex) 
        {
            _appLogger.LogError(
                "An error occurred while sending login alert for user {UserId}. At Date: {Date}",
                ex,
                domainEvent.UserId, DateTime.UtcNow);
            return;
        }
    }
}

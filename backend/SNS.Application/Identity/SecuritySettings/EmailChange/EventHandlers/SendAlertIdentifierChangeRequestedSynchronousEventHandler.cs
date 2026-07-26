using MediatR;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySessions.EventHandlers;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.SecuritySettings.Events;
using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.SecuritySettings.EmailChange.EventHandlers;

internal class SendAlertIdentifierChangeRequestedSynchronousEventHandler :
    INotificationHandler<DomainEventNotification<IdentifierChangeRequestedSynchronousEvent>>
{
    private readonly IEmailSenderService _emailSenderService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly IAppLogger<SendAlertOnHighRiskLoginDetectedEventHandler> _logger;

    public SendAlertIdentifierChangeRequestedSynchronousEventHandler(
        IEmailSenderService emailSenderService,
        IEmailTemplateProvider emailTemplateProvider,
        IAppLogger<SendAlertOnHighRiskLoginDetectedEventHandler> logger)
    {
        _emailSenderService = emailSenderService;
        _emailTemplateProvider = emailTemplateProvider;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<IdentifierChangeRequestedSynchronousEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        try
        {
            var replacement = new List<MessageReplacement>()
            {
                new MessageReplacement( Key: ReplacementKey.UserName, Value: domainEvent.UserName),
                new MessageReplacement( Key: ReplacementKey.Device, Value: domainEvent.Device),
                new MessageReplacement( Key: ReplacementKey.OccuredDate, Value: domainEvent.OccurredOn.ToShortDateString()),
                new MessageReplacement( Key: ReplacementKey.Country, Value: domainEvent.Country),
                new MessageReplacement (Key : ReplacementKey.City, Value : domainEvent.City),
                new MessageReplacement (Key : ReplacementKey.IpAddress, Value : domainEvent.IpAddress),
                new MessageReplacement (Key : ReplacementKey.City, Value : domainEvent.Longitude.ToString()),
                new MessageReplacement (Key : ReplacementKey.Latitude, Value : domainEvent.Latitude.ToString())
            };

            var email = await _emailTemplateProvider.ReadTemplate(domainEvent.SendLanguage, SendPurpose.HighRiskLogin, replacement);

            var sendResult = await _emailSenderService.SendEmailAsync(toEmail: domainEvent.RecipientAddress, subject: email.Subject, message: email.Body);

            if (sendResult.IsFailure)
            {
                _logger.LogError("Failed to send email for user: {UserName}, Operation returned with Status Code: {StatusCode}", new InvalidOperationException(), domainEvent.UserName, sendResult.StatusCode.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while sending a high risk login alert for user: {UserName}, with Id: {UserId}", ex, domainEvent.UserName, domainEvent.UserId);
        }
    }
}

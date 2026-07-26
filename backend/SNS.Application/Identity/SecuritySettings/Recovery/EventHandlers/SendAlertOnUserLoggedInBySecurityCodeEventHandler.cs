using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.SecuritySessions.Events;
using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Identity.SecuritySettings.Recovery.EventHandlers;

public class SendAlertOnUserLoggedInBySecurityCodeEventHandler : INotificationHandler<DomainEventNotification<UserLoggedInBySecurityCodeEvent>>
{
    private readonly IUrlGeneratorService _urlGenerator;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly IEmailSenderService _messageSenderService;

    public SendAlertOnUserLoggedInBySecurityCodeEventHandler(
        IUrlGeneratorService urlGenerator,
        IEmailTemplateProvider emailTemplateProvider,
        IEmailSenderService messageSenderService)
    {
        _urlGenerator = urlGenerator;
        _emailTemplateProvider = emailTemplateProvider;
        _messageSenderService = messageSenderService;
    }


    public async Task Handle(DomainEventNotification<UserLoggedInBySecurityCodeEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var redirectUrl = _urlGenerator.GenerateSecurityEventUrl(domainEvent.UserId);

        var replacements = new List<MessageReplacement>()
        {
            new MessageReplacement (Key: ReplacementKey.IpAddress, Value: domainEvent.IpAddress),
            new MessageReplacement (Key: ReplacementKey.Device, Value: domainEvent.Device),
            new MessageReplacement (Key: ReplacementKey.City, Value: domainEvent.City),
            new MessageReplacement (Key: ReplacementKey.Country, Value: domainEvent.Country),
            new MessageReplacement (Key: ReplacementKey.Longitude, Value: domainEvent.Longitude.ToString()),
            new MessageReplacement (Key: ReplacementKey.Latitude, Value: domainEvent.Latitude.ToString()),
            new MessageReplacement (Key: ReplacementKey.RedirectUrl,Value: redirectUrl)
        };

        var email = await _emailTemplateProvider.ReadTemplate(
            language: domainEvent.SendLanguage,
            replacements: replacements,
            purpose: SendPurpose.LoginWithSecurityCodeAlert);

        await _messageSenderService.SendEmailAsync(
            toEmail: domainEvent.RecipientAddress,
            subject: email.Subject,
            message: email.Body,
            cancellationToken: cancellationToken);
    }
}

using MediatR;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Events;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserBannedEventHandlers;

internal class SendAlertOnUserBannedEventHandler :
    INotificationHandler<DomainEventNotification<UserBannedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly IAppLogger<SendAlertOnUserBannedEventHandler> _logger;

    public SendAlertOnUserBannedEventHandler(
        IApplicationDbContext dbContext,
        IEmailTemplateProvider emailTemplateProvider,
        IEmailSenderService emailSenderService,
        IAppLogger<SendAlertOnUserBannedEventHandler> logger)
    {

        _dbContext = dbContext;
        _emailSenderService = emailSenderService;
        _emailTemplateProvider = emailTemplateProvider;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserBannedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var replacements = new List<MessageReplacement>()
        {
            new MessageReplacement(Key: ReplacementKey.UserName, Value: domainEvent.UserName),
            new MessageReplacement(Key: ReplacementKey.OccuredDate, Value: domainEvent.OccurredOn.ToString("yyyy-MM-dd"))
        };

        var sendResult = Result.Failure(OperationStatusCode.Failure);
        try
        {
            var email = await _emailTemplateProvider.ReadTemplate(
                language: domainEvent.SendLanguage,
                purpose: SendPurpose.UserDeactivated,
                replacements: replacements);

            sendResult = await _emailSenderService.SendEmailAsync(
                toEmail: domainEvent.Email,
                subject: email.Subject,
                message: email.Body,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Error while trying to send a notification for user: {UserId}, after deactivation his account",
                ex);
            throw;
        }
    }
}

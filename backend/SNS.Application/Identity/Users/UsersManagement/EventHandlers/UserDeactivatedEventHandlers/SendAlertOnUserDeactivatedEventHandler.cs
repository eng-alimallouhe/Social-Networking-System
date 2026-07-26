using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Events;
using SNS.Shared.Exceptions;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserDeactivatedEventHandlers;

public class SendAlertOnUserDeactivatedEventHandler :
    INotificationHandler<DomainEventNotification<UserDeactivatedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly IAppLogger<SendAlertOnUserDeactivatedEventHandler> _logger;

    public SendAlertOnUserDeactivatedEventHandler(
        IApplicationDbContext dbContext,
        IEmailTemplateProvider emailTemplateProvider,
        IEmailSenderService emailSenderService,
        IAppLogger<SendAlertOnUserDeactivatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _emailSenderService = emailSenderService;
        _emailTemplateProvider = emailTemplateProvider;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserDeactivatedEvent> notification, CancellationToken cancellationToken)
    {
        var deactivationEvent = notification.DomainEvent;

        var user = await _dbContext.Users
                .Where(u => u.Id == deactivationEvent.UserId)
                .Select(u => new UserBaseDto(
                    Id: u.Id,
                    UserName: u.UserName,
                    DefaultCommunicationMethod: u.UserSecuritySettings.DefaultCommunicationMethod,
                    PreferredLanguage: u.PreferredLanguage,
                    RecoveryEmail: u.UserSecuritySettings.RecoveryEmail,
                    Email: u.Email))
                .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            _logger.LogError(
                "Can't Find user with Id: {UserId} while trying to send a notification for Account deactivation",
                new EntityNotFoundException("Can't Find user with Id: {UserId} while trying to send a notification for Account deactivation"),
                deactivationEvent.UserId);
            return;
        }

        var replacements = new List<MessageReplacement>()
        {
            new MessageReplacement(Key: ReplacementKey.UserName, Value: user.UserName),
            new MessageReplacement(Key: ReplacementKey.Device, Value: deactivationEvent.Device),
            new MessageReplacement(Key: ReplacementKey.IpAddress, Value: deactivationEvent.IpAddress),
            new MessageReplacement(Key: ReplacementKey.Browser, Value: deactivationEvent.Browser),
            new MessageReplacement(Key: ReplacementKey.OccuredDate, Value: deactivationEvent.OccurredOn.ToString("yyyy-MM-dd"))
        };

        var sendResult = Result.Failure(OperationStatusCode.Failure);
        try 
        {
            var email = await _emailTemplateProvider.ReadTemplate(
                language: user.PreferredLanguage,
                purpose: SendPurpose.UserDeactivated,
                replacements: replacements);

            sendResult = await _emailSenderService.SendEmailAsync(
                toEmail: user.RecoveryEmail!,
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
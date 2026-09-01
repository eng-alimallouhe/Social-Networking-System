using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Projects.Events;
using SNS.Shared.Exceptions;

namespace SNS.Application.Projects.EventHandlers.ProjectContributorEventHandlers;

public class SendAlertOnProjectContributorInvitationRespondedEventHandler :
    INotificationHandler<DomainEventNotification<ProjectContributorInvitationRespondedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly IAppLogger<SendAlertOnProjectContributorInvitationRespondedEventHandler> _logger;

    public SendAlertOnProjectContributorInvitationRespondedEventHandler(
        IApplicationDbContext dbContext,
        IEmailTemplateProvider emailTemplateProvider,
        IEmailSenderService emailSenderService,
        IAppLogger<SendAlertOnProjectContributorInvitationRespondedEventHandler> logger)
    {
        _dbContext = dbContext;
        _emailSenderService = emailSenderService;
        _emailTemplateProvider = emailTemplateProvider;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<ProjectContributorInvitationRespondedEvent> notification, CancellationToken cancellationToken)
    {
        var respondedEvent = notification.DomainEvent;

        var projectOwner = await _dbContext.Profiles
            .Where(p => p.Id == respondedEvent.ProjectOwnerProfileId)
            .Select(p => new
            {
                p.Id,
                p.FullName,
                p.Owner.Email,
                p.Owner.PreferredLanguage
            })
            .FirstOrDefaultAsync(cancellationToken);

        var project = await _dbContext.Projects
            .Where(p => p.Id == respondedEvent.ProjectId)
            .Select(p => p.Title)
            .FirstOrDefaultAsync(cancellationToken);

        if (projectOwner == null || project == null)
        {
            _logger.LogError(
                "Can't Find project owner or project while trying to send project invitation response email",
                new EntityNotFoundException("Can't Find project owner or project while trying to send project invitation response email"),
                respondedEvent.ProjectOwnerProfileId);
            return;
        }

        var statusString = respondedEvent.IsAccepted ? "accepted" : "rejected";

        var replacements = new List<MessageReplacement>()
        {
            new MessageReplacement(Key: ReplacementKey.ProjectOwnerName, Value: projectOwner.FullName),
            new MessageReplacement(Key: ReplacementKey.InvitedUserName, Value: respondedEvent.InvitedUserName),
            new MessageReplacement(Key: ReplacementKey.ProjectName, Value: project),
            new MessageReplacement(Key: ReplacementKey.Status, Value: statusString)
        };

        try
        {
            var email = await _emailTemplateProvider.ReadTemplate(
                language: projectOwner.PreferredLanguage,
                purpose: SendPurpose.ProjectContributorInvitationResponse,
                replacements: replacements);

            await _emailSenderService.SendEmailAsync(
                toEmail: projectOwner.Email!,
                subject: email.Subject,
                message: email.Body,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Error while trying to send a project invitation response email to project owner: {ProfileId}",
                ex);
            throw;
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Application.Shared.Events;
using SNS.Application.Shared.Settings;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Projects.Events;
using SNS.Shared.Exceptions;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Projects.EventHandlers.ProjectContributorEventHandlers;

public class SendInvitationEmailOnProjectContributorInvitationSentEventHandler :
    INotificationHandler<DomainEventNotification<ProjectContributorInvitationSentEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly IAppLogger<SendInvitationEmailOnProjectContributorInvitationSentEventHandler> _logger;
    private readonly AppSettings _appSettings;

    public SendInvitationEmailOnProjectContributorInvitationSentEventHandler(
        IApplicationDbContext dbContext,
        IEmailTemplateProvider emailTemplateProvider,
        IEmailSenderService emailSenderService,
        IAppLogger<SendInvitationEmailOnProjectContributorInvitationSentEventHandler> logger,
        IOptions<AppSettings> appSettings)
    {
        _dbContext = dbContext;
        _emailSenderService = emailSenderService;
        _emailTemplateProvider = emailTemplateProvider;
        _logger = logger;
        _appSettings = appSettings.Value;
    }

    public async Task Handle(DomainEventNotification<ProjectContributorInvitationSentEvent> notification, CancellationToken cancellationToken)
    {
        var invitationEvent = notification.DomainEvent;

        var invitedUser = await _dbContext.Profiles
            .Where(p => p.Id == invitationEvent.InvitedProfileId)
            .Select(p => new
            {
                p.Id,
                p.FullName,
                p.Owner.Email,
                p.Owner.PreferredLanguage,
                p.Owner.UserSecuritySettings.DefaultCommunicationMethod
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (invitedUser == null)
        {
            _logger.LogError(
                "Can't Find profile with Id: {ProfileId} while trying to send project invitation email",
                new EntityNotFoundException("Can't Find profile with Id: {ProfileId} while trying to send project invitation email"),
                invitationEvent.InvitedProfileId);
            return;
        }

        var invitationUrl = $"{_appSettings.ClientUrl}/projects/invitations"; // Or adjust path as needed for frontend

        var replacements = new List<MessageReplacement>()
        {
            new MessageReplacement(Key: ReplacementKey.RecipientName, Value: invitedUser.FullName),
            new MessageReplacement(Key: ReplacementKey.ProjectName, Value: invitationEvent.ProjectName),
            new MessageReplacement(Key: ReplacementKey.ProjectOwnerName, Value: invitationEvent.ProjectOwnerName),
            new MessageReplacement(Key: ReplacementKey.ProjectOwnerProfileImageUrl, Value: invitationEvent.ProjectOwnerProfileImageUrl ?? _appSettings.LogoUrl),
            new MessageReplacement(Key: ReplacementKey.InvitationUrl, Value: invitationUrl)
        };

        try
        {
            var email = await _emailTemplateProvider.ReadTemplate(
                language: invitedUser.PreferredLanguage,
                purpose: SendPurpose.ProjectContributorInvitation,
                replacements: replacements);

            await _emailSenderService.SendEmailAsync(
                toEmail: invitedUser.Email!,
                subject: email.Subject,
                message: email.Body,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Error while trying to send a project invitation email to user associated with profile: {ProfileId}",
                ex);
            throw;
        }
    }
}

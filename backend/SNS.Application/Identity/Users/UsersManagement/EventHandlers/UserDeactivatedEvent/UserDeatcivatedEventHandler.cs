using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Contracts.Messaging;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Exceptions;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserDeactivatedEvent;

public class DocumentUserDeatcivatedEventHandler : INotificationHandler<DomainEventNotification<UserDeactivatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPostSearchService _postSearchService;
    private readonly IProblemSearchService _problemSearchService;
    private readonly IProfileSearchService _profileSearchService;
    private readonly IUserSearchService _userSearchService;
    private readonly IProjectSearchService _projectSearchService;
    private readonly IAppLogger<DocumentUserDeatcivatedEventHandler> _logger;

    public DocumentUserDeatcivatedEventHandler(
        IApplicationDbContext dbContext,
        IPostSearchService postSearchService,
        IProblemSearchService problemSearchService,
        IProfileSearchService profileSearchService,
        IUserSearchService userSearchService,
        IProjectSearchService projectSearchService,
        IAppLogger<DocumentUserDeatcivatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _postSearchService = postSearchService;
        _problemSearchService = problemSearchService;
        _profileSearchService = profileSearchService;
        _userSearchService = userSearchService;
        _projectSearchService = projectSearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserDeactivatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        Guid? profileId = await _dbContext.Profiles
                .Where(p => p.UserId == notification.DomainEvent.UserId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync(cancellationToken);

        if (profileId == null)
        {
            _logger.LogWarning("Can't Find Profile that is related for user: {UserId}", notification.DomainEvent.UserId);
            return;
        }

        var problemDeletingTask = _problemSearchService.DeleteProblemsByAuthorIdAsync(profileId.Value, cancellationToken);
        var projectsDeletingTask = _projectSearchService.DeleteProjectsByOnwerIdAsync(profileId.Value, cancellationToken);
        var postsDeletingTask = _postSearchService.DeletePostsByAuthorIdAsync(profileId.Value, cancellationToken);
        var userDeletingTask = _userSearchService.DeleteUserAsync(notification.DomainEvent.UserId, cancellationToken);
        var profileDeletingTask = _profileSearchService.DeleteProfile(profileId.Value, cancellationToken);
        try 
        { 
            await Task.WhenAll(problemDeletingTask, projectsDeletingTask, postsDeletingTask, profileDeletingTask, userDeletingTask);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Can't Delte User ContentManagement after deactivate his Account, User Id : {UserId}",
                ex,
                notification.DomainEvent.UserId);
            throw;
        }
    }
}

public class UserDeactivatedLockProfileEventHandler :
    INotificationHandler<DomainEventNotification<UserDeactivatedIntegrationEvent>>
{
    private readonly ISoftDeletableRepository<Profile> _profileRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger<UserDeactivatedLockProfileEventHandler> _logger;

    public UserDeactivatedLockProfileEventHandler(
        ISoftDeletableRepository<Profile> profileRepo, IUnitOfWork unitOfWork, IAppLogger<UserDeactivatedLockProfileEventHandler> logger)
    {
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserDeactivatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var profile = await _profileRepo.GetSingleByExpressionAsync(
            p => p.UserId == notification.DomainEvent.UserId, cancellationToken);

        if (profile == null)
        {
            _logger.LogError(
                "Can't Find the profile for user with Id: {UserId}",
                new EntityNotFoundException(""),
                notification.DomainEvent.UserId);
            return;
        }
        try {
            profile.SoftDelete();

            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Error while deactivation the profile for user: {UserId}", ex, notification.DomainEvent.UserId);
            throw;
        }
    }
}

public class UserDeactivatedNotificationEventHandler :
    INotificationHandler<DomainEventNotification<UserDeactivatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IEmailTemplateProvider _emailTemplateProvider;
    private readonly IAppLogger<UserDeactivatedNotificationEventHandler> _logger;

    public UserDeactivatedNotificationEventHandler(
        IApplicationDbContext dbContext,
        IEmailTemplateProvider emailTemplateProvider,
        IEmailSenderService emailSenderService,
        IAppLogger<UserDeactivatedNotificationEventHandler> logger)
    {
        _dbContext = dbContext;
        _emailSenderService = emailSenderService;
        _emailTemplateProvider = emailTemplateProvider;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserDeactivatedIntegrationEvent> notification, CancellationToken cancellationToken)
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
            new MessageReplacement(Key: ReplacementKey.OccuredDate, Value: deactivationEvent.DeactivatedAt.ToString("yyyy-MM-dd"))
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
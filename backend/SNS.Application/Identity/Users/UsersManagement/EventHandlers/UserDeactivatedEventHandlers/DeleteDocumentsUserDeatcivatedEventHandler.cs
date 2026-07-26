using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Identity.Users.Events;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserDeactivatedEventHandlers;

public class DeleteDocumentsUserDeatcivatedEventHandler : INotificationHandler<DomainEventNotification<Domain.Identity.Users.Events.UserDeactivatedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPostSearchService _postSearchService;
    private readonly IProblemSearchService _problemSearchService;
    private readonly IProfileSearchService _profileSearchService;
    private readonly IUserSearchService _userSearchService;
    private readonly IProjectSearchService _projectSearchService;
    private readonly IAppLogger<DeleteDocumentsUserDeatcivatedEventHandler> _logger;

    public DeleteDocumentsUserDeatcivatedEventHandler(
        IApplicationDbContext dbContext,
        IPostSearchService postSearchService,
        IProblemSearchService problemSearchService,
        IProfileSearchService profileSearchService,
        IUserSearchService userSearchService,
        IProjectSearchService projectSearchService,
        IAppLogger<DeleteDocumentsUserDeatcivatedEventHandler> logger)
    {
        _dbContext = dbContext;
        _postSearchService = postSearchService;
        _problemSearchService = problemSearchService;
        _profileSearchService = profileSearchService;
        _userSearchService = userSearchService;
        _projectSearchService = projectSearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<Domain.Identity.Users.Events.UserDeactivatedEvent> notification, CancellationToken cancellationToken)
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
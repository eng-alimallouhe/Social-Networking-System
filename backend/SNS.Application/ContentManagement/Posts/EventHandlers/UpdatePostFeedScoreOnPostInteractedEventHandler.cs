using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Posts.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Posts.Enums;
using SNS.Domain.ContentManagement.Posts.Events;
using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Application.ContentManagement.Posts.EventHandlers;

public class UpdatePostFeedScoreOnPostInteractedEventHandler
    : INotificationHandler<DomainEventNotification<PostInteractedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPostCacheService _postCacheService;
    private readonly IRepository<ProfileTopic> _profileTopicRepository;


    public UpdatePostFeedScoreOnPostInteractedEventHandler(
        IApplicationDbContext dbContext,
        IPostCacheService postCacheService,
        IRepository<ProfileTopic> profileTopicRepository)
    {
        _dbContext = dbContext;
        _postCacheService = postCacheService;
        _profileTopicRepository = profileTopicRepository;
    }

    public async Task Handle(DomainEventNotification<PostInteractedEvent> notification, CancellationToken cancellationToken)
    {
        var postId = notification.DomainEvent.PostId;
        var type = notification.DomainEvent.Type;
        var profileId = notification.DomainEvent.ProfileId;

        var topicIds = await _dbContext.PostTopics
            .Where(pt => pt.PostId == postId)
            .Select(pt => pt.TopicId)
            .ToListAsync();

        double weightChange = type switch
        {
            InteractionType.Like => 0.2,
            InteractionType.Comment => 0.5,
            InteractionType.NotInterested => -1.0,
            _ => 0.0
        };

        var existingTopics = await _profileTopicRepository.GetListByExpressionAsync(
            pt => topicIds.Contains(pt.TopicId) && pt.ProfileId == profileId);

        var existingDictionary = existingTopics
            .ToDictionary(x => x.TopicId);

        foreach (var topicId in topicIds)
        {
            if (existingDictionary.TryGetValue(topicId, out var profileTopic))
            {
                profileTopic.UpdateScore(weightChange);
            }
            else if (type != InteractionType.NotInterested)
            {
                _profileTopicRepository.Add(ProfileTopic.Create(
                    profileId: profileId,
                    topicId: topicId,
                    score: 1.0
                ));
            }
        }
    }
}

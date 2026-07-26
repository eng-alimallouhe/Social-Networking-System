using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.ContentManagement.Posts.Contracts;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Shared.Abstractions.AI;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.ContentManagement.Posts.Events;
using SNS.Domain.Search.Documents;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Exceptions;

namespace SNS.Application.ContentManagement.Posts.EventHandlers;



public class ClassifyTopicsOnPostCreatedEventHandler
    : INotificationHandler<DomainEventNotification<PostCreatedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITopicClassifier _topicClassifier;
    private readonly IPostSearchService _postSearchService;
    private readonly IRepository<PostTopic> _postTopicRepo;
    private readonly IAppLogger<ClassifyTopicsOnPostCreatedEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public ClassifyTopicsOnPostCreatedEventHandler(
        IApplicationDbContext dbContext,
        ITopicClassifier topicClassifier,
        IPostSearchService postSearchService,
        IRepository<PostTopic> postTopicRepo,
        IAppLogger<ClassifyTopicsOnPostCreatedEventHandler> logger,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _topicClassifier = topicClassifier;
        _postSearchService = postSearchService;
        _postTopicRepo = postTopicRepo;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(DomainEventNotification<PostCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        // Steps: 
        // 1. Get the post data 
        // 2. Send To the model to classify topics 
        // 3. ensure the topics are found in the DB
        // 4. add post to search layer
        // 5. add post topics to PostTopics table

        try
        {
            var post = await _dbContext
                .Posts
                .Where(p => p.Id == domainEvent.PostId)
                .Select(p => new PostToClassify
                {
                    Id = p.Id,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.FullName,
                    AuthorSpecialization = p.Author.Specialization,
                    AuthorProfilePictureObjectKey = p.Author.ProfilePictureObjectKey,
                    CommunityId = p.CommunityId,
                    CommunityType = p.Community == null? null : p.Community.Type,
                    CommunityName = p.Community == null? null : p.Community.Name,
                    CommunityLogoObjectKey = p.Community == null? null : p.Community.LogoObjectKey,
                    Title = p.Title,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    LastInteractedAt = p.LastInteractedAt,
                    Medias = p.Media.Select(m => new MediaSnapshot
                    {
                        ObjectKey = m.ObjectKey,
                        MediaType = m.Type
                    })
                    .ToList(),
                    Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                    CommentsCount = p.Comments.Count(),
                    ReactionsCount = p.Reactions.Count(),
                    ViewsCount = p.Views.Count(),
                    SavesCount = p.SavedPosts.Count()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (post == null)
            {
                _logger.LogError("Can not find post with Id: {postId} to classify with Occurred Date: {OccurredOn}", new ResourceNotFoundException(), domainEvent.PostId, domainEvent.OccurredOn);
                return;
            }

            var classificationResult =  await _topicClassifier.DetectTopicsAsync(new PostAnalysisRequest(
                Content: post.Content,
                Videos: post.Medias
                    .Where(m => m.MediaType == MediaType.Video)
                    .Select(m => _fileStorageService.GetFilePublicUrl(m.ObjectKey))
                    .ToList(),
                Images: post.Medias
                    .Where(m => m.MediaType == MediaType.Image)
                    .Select(m => _fileStorageService.GetFilePublicUrl(m.ObjectKey))
                    .ToList()));

            var slugs = classificationResult
                .Select(t => t.slug)
                .Distinct()
                .ToList();

            var topics = await _dbContext.Topics
                .Where(t => slugs.Contains(t.Name))
                .ToDictionaryAsync(
                    t => t.Name,
                    cancellationToken);

            var postTopics = new List<PostTopic>();

            foreach (var prediction in classificationResult)
            {
                if (topics.TryGetValue(prediction.slug, out var topic))
                {
                    postTopics.Add(PostTopic.Create(
                        postId: post.Id,
                        topicId: topic.Id,
                        confidence: prediction.Confidence));
                }
                else
                {
                    _logger.LogWarning(
                        "Unknown topic returned from AI: {Slug}",
                        prediction.slug);
                }
            }

            _postTopicRepo.AddRange(postTopics);

            await _unitOfWork.CompleteAsync(cancellationToken);

            var postDocument = new PostDocument
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                AuthorName = post.AuthorName,
                AuthorSpecialization = post.AuthorSpecialization,
                AuthorProfilePictureObjectKey = post.AuthorProfilePictureObjectKey,
                CommunityId = post.CommunityId,
                CommunityType = post.CommunityType,
                CommunityName = post.CommunityName,
                CommunityLogoObjectKey = post.CommunityLogoObjectKey,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LastInteractedAt = post.LastInteractedAt,
                MediaUrls = post.Medias.Select(m => _fileStorageService.GetFilePublicUrl(m.ObjectKey)).ToList(),
                Tags = post.Tags.ToList(),
                CommentsCount = post.CommentsCount,
                ReactionsCount = post.ReactionsCount,
                ViewsCount = post.ViewsCount,
                SavesCount = post.SavesCount,
                Topics = postTopics
                    .Select(pt => topics.First(t => t.Value.Id == pt.TopicId).Key)
                    .ToList()
            };

            await _postSearchService.UpsertPostAsync(postDocument, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                "Failed to classify post {PostId}",
                ex,
                domainEvent.PostId);
        }

    }
}

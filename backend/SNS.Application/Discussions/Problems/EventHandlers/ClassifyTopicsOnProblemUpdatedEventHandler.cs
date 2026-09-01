using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Discussions.Problems.Contracts;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Shared.Abstractions.AI;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Application.Shared.Events;
using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Problems.Events;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Search.Documents;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Exceptions;

namespace SNS.Application.Discussions.Problems.EventHandlers;

/// <summary>
/// Handles <see cref="ProblemUpdatedEvent"/> to reclassify discussion problem topics using AI when required and sync the updated problem with the search index.
/// </summary>
public class ClassifyTopicsOnProblemUpdatedEventHandler
    : INotificationHandler<DomainEventNotification<ProblemUpdatedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ITopicClassifier _topicClassifier;
    private readonly IProblemSearchService _problemSearchService;
    private readonly IRepository<ProblemTopic> _problemTopicRepo;
    private readonly IAppLogger<ClassifyTopicsOnProblemUpdatedEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public ClassifyTopicsOnProblemUpdatedEventHandler(
        IApplicationDbContext dbContext,
        ITopicClassifier topicClassifier,
        IProblemSearchService problemSearchService,
        IRepository<ProblemTopic> problemTopicRepo,
        IAppLogger<ClassifyTopicsOnProblemUpdatedEventHandler> logger,
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _topicClassifier = topicClassifier;
        _problemSearchService = problemSearchService;
        _problemTopicRepo = problemTopicRepo;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task Handle(DomainEventNotification<ProblemUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var problem = await _dbContext
            .Problems
            .Where(p => p.Id == domainEvent.ProblemId)
            .Select(p => new ProblemToClassify
            {
                Id = p.Id,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.FullName,
                AuthorSpecialization = p.Author.Specialization,
                AuthorProfilePictureObjectKey = p.Author.ProfilePictureObjectKey,
                CommunityId = p.CommunityId,
                Title = p.Title,
                Status = p.Status,
                Level = p.Level,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                ContentBlocks = p.ContentBlocks
                    .OrderBy(b => b.Order)
                    .Select(b => new ProblemContentBlockSnapshot
                    {
                        Type = b.Type,
                        Content = b.Content,
                        ExtraInfo = b.ExtraInfo,
                        Order = b.Order
                    })
                    .ToList(),
                Tags = p.ProblemTags.Select(pt => pt.Tag.Name).ToList(),
                Topics = p.ProblemTopics.Select(pt => pt.Topic.Name).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (problem == null)
        {
            _logger.LogError("Can not find problem with Id: {problemId} to classify with Occurred Date: {OccurredOn}", new ResourceNotFoundException(), domainEvent.ProblemId, domainEvent.OccurredOn);
            return;
        }

        if (domainEvent.RequiresReclassification)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var classificationResult = await _topicClassifier.DetectTopicsAsync(new ProblemAnalysisRequest(
                    Title: problem.Title,
                    Codes: problem.ContentBlocks
                        .Where(b => b.Type == ProblemBlockType.Code)
                        .OrderBy(b => b.Order)
                        .Select(b => b.Content)
                        .ToList(),
                    TextBlocks: problem.ContentBlocks
                        .Where(b => b.Type == ProblemBlockType.Text)
                        .OrderBy(b => b.Order)
                        .Select(b => b.Content)
                        .ToList(),
                    Videos: problem.ContentBlocks
                        .Where(b => b.Type == ProblemBlockType.Video)
                        .OrderBy(b => b.Order)
                        .Select(b => _fileStorageService.GetFilePublicUrl(b.Content))
                        .ToList(),
                    Images: problem.ContentBlocks
                        .Where(b => b.Type == ProblemBlockType.Image)
                        .OrderBy(b => b.Order)
                        .Select(b => _fileStorageService.GetFilePublicUrl(b.Content))
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

                var problemTopics = new List<ProblemTopic>();

                foreach (var prediction in classificationResult)
                {
                    if (topics.TryGetValue(prediction.slug, out var topic))
                    {
                        problemTopics.Add(ProblemTopic.Create(
                            problemId: problem.Id,
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

                await _problemTopicRepo.ExecuteDeleteAsync(
                    pt => pt.ProblemId == problem.Id,
                    cancellationToken);

                _problemTopicRepo.AddRange(problemTopics);

                var topicsById = topics.Values.ToDictionary(t => t.Id);

                problem.Topics = problemTopics
                    .Select(pt => topicsById[pt.TopicId].Name)
                    .ToList();

                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                _logger.LogError(
                    "Failed to classify problem {ProblemId}",
                    ex,
                    domainEvent.ProblemId);
            }
        }

        var problemDocument = new ProblemDocument
        {
            Id = problem.Id,
            Title = problem.Title,
            Status = problem.Status,
            Level = problem.Level,
            ContentBlocks = problem.ContentBlocks
                .OrderBy(cb => cb.Order)
                .Select(cb => new ProblemBlockDocument
                {
                    Type = cb.Type,
                    Content = cb.Content,
                    ExtraInfo = cb.ExtraInfo,
                    Order = cb.Order
                })
                .ToList(),
            CreatedAt = problem.CreatedAt,
            UpdatedAt = problem.UpdatedAt,
            Tags = problem.Tags.ToList(),
            Topics = problem.Topics
        };

        await _problemSearchService.UpsertProblemAsync(problemDocument, cancellationToken);
    }
}

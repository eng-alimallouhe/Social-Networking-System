using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.Posts.Commands.IncreaseInterest;

public sealed record IncreasePostInterestCommand(Guid PostId) : ICommand;

internal sealed class IncreasePostInterestCommandHandler : ICommandHandler<IncreasePostInterestCommand>
{
    private readonly IRepository<ProfileTopic> _profileTopicRepo;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public IncreasePostInterestCommandHandler(
        IRepository<ProfileTopic> profileTopicRepo,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _profileTopicRepo = profileTopicRepo;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(IncreasePostInterestCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var topicIds = await _dbContext.PostTopics
            .AsNoTracking()
            .Where(pt => pt.PostId == request.PostId)
            .Select(pt => pt.TopicId)
            .ToListAsync(cancellationToken);

        if (!topicIds.Any())
        {
            return Result.Success(OperationStatusCode.Success);
        }

        var profileTopics = await _profileTopicRepo.GetListByExpressionAsync(
            pt => pt.ProfileId == profileId.Value && topicIds.Contains(pt.TopicId),
            cancellationToken);

        var existingTopicIds = new HashSet<Guid>();

        foreach (var profileTopic in profileTopics)
        {
            profileTopic.UpdateScore(1);
            existingTopicIds.Add(profileTopic.TopicId);
        }

        var newTopicIds = topicIds.Except(existingTopicIds);

        foreach (var newTopicId in newTopicIds)
        {
            var newProfileTopic = ProfileTopic.Create(profileId.Value, newTopicId, 1);
            _profileTopicRepo.Add(newProfileTopic);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.SocialGraph.Abstractions;
using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Profiles;

namespace SNS.Application.Profiles.SocialGraph.Commands.FollowProfile;

/// <summary>
/// Handles the execution of <see cref="FollowProfileCommand"/> to follow a user profile.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated follower profile ID.
/// 2. Checks social relationship policy rules using <see cref="ISocialPolicyService"/>.
/// 3. Creates a new <see cref="Follow"/> entity.
/// 4. Saves the follow relationship entity to database.
/// Side effects include follow entity creation and database persistence.
/// </remarks>
internal sealed class FollowProfileCommandHandler
    : ICommandHandler<FollowProfileCommand>
{
    private readonly ISocialPolicyService _socialPolicyService;
    private readonly IRepository<Follow> _followRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public FollowProfileCommandHandler(
        ISocialPolicyService socialPolicyService,
        IRepository<Follow> followRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _socialPolicyService = socialPolicyService;
        _followRepo = followRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(FollowProfileCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(ProfileStatusCodes.NotFound);
        }

        var isRelationshipAllowedResult = await _socialPolicyService.IsRelationshipAllowedAsync(
            firstRelationshipPart: profileId.Value, 
            secondRelationshipPart: request.TargetProfileId);

        if (isRelationshipAllowedResult.IsFailure && isRelationshipAllowedResult.StatusCode == ProfileStatusCodes.NotFound)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }
        else if (isRelationshipAllowedResult.IsFailure)
        {
            return isRelationshipAllowedResult;
        }

        _followRepo.Add(Follow.Create(
            followerId: profileId.Value,
            followingId: request.TargetProfileId));

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }
}
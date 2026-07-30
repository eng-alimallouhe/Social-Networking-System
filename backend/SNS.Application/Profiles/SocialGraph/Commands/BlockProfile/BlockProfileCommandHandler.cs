using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Profiles;

namespace SNS.Application.Profiles.SocialGraph.Commands.BlockProfile;

/// <summary>
/// Handles the execution of <see cref="BlockProfileCommand"/> to block a profile.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves current authenticated profile ID.
/// 2. Validates that the target profile is not self.
/// 3. Verifies whether a block already exists.
/// 4. Removes any existing mutual follow records between the two profiles.
/// 5. Adds a new <see cref="Block"/> entity and commits database changes.
/// Side effects include removing follow entities, creating a block entity, and database transaction commit.
/// </remarks>
internal class BlockProfileCommandHandler
    : ICommandHandler<BlockProfileCommand>
{
    private readonly IRepository<Follow> _followRepo;
    private readonly IRepository<Block> _blockRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public BlockProfileCommandHandler(
        IRepository<Follow> followRepo,
        IRepository<Block> blockRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _followRepo = followRepo;
        _blockRepo = blockRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(BlockProfileCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(ProfileStatusCodes.NotFound);
        }

        if (request.TargetProfileId == profileId.Value)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }

        var isBlockedAlready = await _blockRepo.ExistsAsync(
            b => b.BlockedId == request.TargetProfileId && b.BlockerId == profileId.Value, cancellationToken);

        if (isBlockedAlready)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        var followCount = await _followRepo.ExecuteDeleteAsync(
            f => 
            (f.FollowerId == request.TargetProfileId && f.FollowingId == profileId.Value) || 
            (f.FollowerId == profileId.Value && f.FollowingId == request.TargetProfileId), cancellationToken);

        _blockRepo.Add(Block.Create(
            blockedId: request.TargetProfileId,
            blockerId: profileId.Value));

        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }
}

using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Memberships.Commands.LeaveCommunity;

/// <summary>
/// Represents a command to leave an existing community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
public sealed record LeaveCommunityCommand(
    Guid CommunityId
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="LeaveCommunityCommand"/> to remove user membership from a community.
/// </summary>
internal sealed class LeaveCommunityCommandHandler : ICommandHandler<LeaveCommunityCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<CommunityMembership> _membershipRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public LeaveCommunityCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<CommunityMembership> membershipRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _membershipRepo = membershipRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(LeaveCommunityCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var community = await _dbContext.Communities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CommunityId && c.IsActive, cancellationToken);

        if (community == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        if (community.OwnerId == profileId.Value)
        {
            return Result.Failure(OperationStatusCode.Failure);
        }

        var membership = await _membershipRepo.GetSingleByExpressionAsync(
            m => m.CommunityId == request.CommunityId && m.MemberId == profileId.Value, cancellationToken);

        if (membership == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        _membershipRepo.Delete(membership);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

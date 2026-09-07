using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Memberships.Commands.CancelJoinRequest;

/// <summary>
/// Represents a command for the current authenticated user to cancel their own pending community join request.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
public sealed record CancelJoinRequestCommand(
    Guid CommunityId
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="CancelJoinRequestCommand"/> to cancel a pending join request.
/// </summary>
internal sealed class CancelJoinRequestCommandHandler : ICommandHandler<CancelJoinRequestCommand>
{
    private readonly IRepository<CommunityJoinRequest> _joinRequestRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CancelJoinRequestCommandHandler(
        IRepository<CommunityJoinRequest> joinRequestRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _joinRequestRepo = joinRequestRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(CancelJoinRequestCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var joinRequest = await _joinRequestRepo.GetSingleByExpressionAsync(
            r => r.CommunityId == request.CommunityId &&
                 r.SubmitterId == profileId.Value &&
                 r.Status == JoinRequestStatus.Pending,
            cancellationToken);

        if (joinRequest == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        joinRequest.Cancel();
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

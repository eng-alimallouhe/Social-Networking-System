using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.PostSaves.Commands.UnsavePost;

public sealed record UnsavePostCommand(
    Guid PostId
) : ICommand;

internal sealed class UnsavePostCommandHandler : ICommandHandler<UnsavePostCommand>
{
    private readonly IRepository<SavedPost> _savedPostRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UnsavePostCommandHandler(
        IRepository<SavedPost> savedPostRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _savedPostRepo = savedPostRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UnsavePostCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var existingSavedPost = await _savedPostRepo.GetSingleByExpressionAsync(
            sp => sp.PostId == request.PostId && sp.ProfileId == profileId.Value, cancellationToken);

        if (existingSavedPost == null)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        _savedPostRepo.Delete(existingSavedPost);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.PostSaves.Commands.SavePost;

public sealed record SavePostCommand(
    Guid PostId
) : ICommand;

internal sealed class SavePostCommandHandler : ICommandHandler<SavePostCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<SavedPost> _savedPostRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SavePostCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<SavedPost> savedPostRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _savedPostRepo = savedPostRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(SavePostCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var postExists = await _dbContext.Posts
            .AnyAsync(p => p.Id == request.PostId && p.IsActive, cancellationToken);

        if (!postExists)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var isAlreadySaved = await _savedPostRepo.ExistsAsync(
            sp => sp.PostId == request.PostId && sp.ProfileId == profileId.Value, cancellationToken);

        if (isAlreadySaved)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        var savedPost = SavedPost.Create(profileId.Value, request.PostId);
        _savedPostRepo.Add(savedPost);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

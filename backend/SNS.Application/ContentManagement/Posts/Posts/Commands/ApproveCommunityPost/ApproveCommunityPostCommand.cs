using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.ContentManagement.Posts.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Posts.Posts.Commands.ApproveCommunityPost;

/// <summary>
/// Represents a command to approve a pending post published in a community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="PostId">The unique identifier of the post.</param>
public sealed record ApproveCommunityPostCommand(
    Guid CommunityId,
    Guid PostId
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="ApproveCommunityPostCommand"/> to approve a pending post.
/// </summary>
internal sealed class ApproveCommunityPostCommandHandler : ICommandHandler<ApproveCommunityPostCommand>
{
    private readonly ISoftDeletableRepository<Post> _postRepo;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public ApproveCommunityPostCommandHandler(
        ISoftDeletableRepository<Post> postRepo,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _postRepo = postRepo;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(ApproveCommunityPostCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var community = await _dbContext.Communities
            .AsNoTracking()
            .Where(c => c.Id == request.CommunityId && c.IsActive)
            .Select(c => new { c.Id, c.OwnerId })
            .FirstOrDefaultAsync(cancellationToken);

        if (community == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var isOwner = community.OwnerId == profileId.Value;
        var isModerator = !isOwner && await _dbContext.CommunityMemberships
            .AnyAsync(m => m.CommunityId == request.CommunityId &&
                           m.MemberId == profileId.Value &&
                           (m.Role == CommunityRole.Moderator || m.Role == CommunityRole.Owner) &&
                           m.Status == CommunityMembershipStatus.Active, cancellationToken);

        if (!isOwner && !isModerator)
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        // Load tracked post entity through repository
        var post = await _postRepo.GetSingleByExpressionAsync(
            p => p.Id == request.PostId &&
                 p.CommunityId == request.CommunityId &&
                 p.IsActive,
            cancellationToken);

        if (post == null || post.Status != PostStatus.Pending)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        post.ChangeStatus(PostStatus.Accepted);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.ContentManagement.Posts.PostMentions.Helpers;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Shared.StatusCodes.Profiles;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Comments.Comments.Commands.UpdateComment;

public sealed record UpdateCommentCommand(
    Guid CommentId,
    string Content,
    List<Guid>? MentionedProfileIds = null
) : ICommand;

internal sealed class UpdateCommentCommandHandler : ICommandHandler<UpdateCommentCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ISoftDeletableRepository<Comment> _commentRepo;
    private readonly IRepository<CommentMention> _commentMentionRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCommentCommandHandler(
        IApplicationDbContext dbContext,
        ISoftDeletableRepository<Comment> commentRepo,
        IRepository<CommentMention> commentMentionRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _commentRepo = commentRepo;
        _commentMentionRepo = commentMentionRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
        }

        var comment = await _commentRepo.GetByIdAsync(request.CommentId, cancellationToken);
        if (comment == null || !comment.IsActive)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        if (comment.AuthorId != profileId.Value)
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        // Validate mentions
        var contentMentionIds = MentionParser.ExtractMentionedProfileIds(request.Content);
        if (request.MentionedProfileIds != null && request.MentionedProfileIds.Any())
        {
            if (!request.MentionedProfileIds.All(id => contentMentionIds.Contains(id)))
            {
                return Result.Failure(OperationStatusCode.InvalidInput);
            }
        }

        if (contentMentionIds.Any())
        {
            var activeProfilesCount = await _dbContext.Profiles
                .CountAsync(p => contentMentionIds.Contains(p.Id) && p.IsActive, cancellationToken);

            if (activeProfilesCount != contentMentionIds.Count)
            {
                return Result.Failure(ProfileStatusCodes.NotFound);
            }
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            comment.UpdateContent(request.Content);

            var existingMentions = await _dbContext.CommentMentions
                .Where(m => m.CommentId == comment.Id)
                .ToListAsync(cancellationToken);

            _commentMentionRepo.DeleteRange(existingMentions);

            foreach (var mentionedId in contentMentionIds)
            {
                var mention = new CommentMention
                {
                    CommentId = comment.Id,
                    ProfileId = mentionedId
                };
                _commentMentionRepo.Add(mention);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success(OperationStatusCode.Success);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}

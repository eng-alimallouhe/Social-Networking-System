using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.ContentManagement.Communities.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.ContentManagement.Communities.Rules.Commands.DeleteCommunityRule;

/// <summary>
/// Represents a command to delete a rule from a community.
/// </summary>
/// <param name="RuleId">The unique identifier of the rule to delete.</param>
public sealed record DeleteCommunityRuleCommand(
    Guid RuleId
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="DeleteCommunityRuleCommand"/> to remove a community rule.
/// </summary>
internal sealed class DeleteCommunityRuleCommandHandler : ICommandHandler<DeleteCommunityRuleCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<CommunityRule> _ruleRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteCommunityRuleCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<CommunityRule> ruleRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _ruleRepo = ruleRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(DeleteCommunityRuleCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var rule = await _ruleRepo.GetByIdAsync(request.RuleId, cancellationToken);
        if (rule == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var community = await _dbContext.Communities
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == rule.CommunityId && c.IsActive, cancellationToken);

        if (community == null)
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var isOwner = community.OwnerId == profileId.Value;
        var isModerator = !isOwner && await _dbContext.CommunityMemberships
            .AnyAsync(m => m.CommunityId == community.Id &&
                           m.MemberId == profileId.Value &&
                           (m.Role == CommunityRole.Moderator || m.Role == CommunityRole.Owner) &&
                           m.Status == CommunityMembershipStatus.Active, cancellationToken);

        if (!isOwner && !isModerator)
        {
            return Result.Failure(SecurityStatusCodes.UnAuthorized);
        }

        _ruleRepo.Delete(rule);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

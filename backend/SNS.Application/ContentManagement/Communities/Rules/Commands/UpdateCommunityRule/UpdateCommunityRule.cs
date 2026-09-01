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

namespace SNS.Application.ContentManagement.Communities.Rules.Commands.UpdateCommunityRule;

/// <summary>
/// Represents a command to update an existing community rule.
/// </summary>
/// <param name="RuleId">The unique identifier of the rule.</param>
/// <param name="Title">The updated rule title.</param>
/// <param name="Description">The updated rule description.</param>
/// <param name="Order">The updated display order.</param>
public sealed record UpdateCommunityRuleCommand(
    Guid RuleId,
    string Title,
    string Description,
    int Order
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdateCommunityRuleCommand"/> to modify an existing community rule.
/// </summary>
internal sealed class UpdateCommunityRuleCommandHandler : ICommandHandler<UpdateCommunityRuleCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<CommunityRule> _ruleRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateCommunityRuleCommandHandler(
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

    public async Task<Result> Handle(UpdateCommunityRuleCommand request, CancellationToken cancellationToken)
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

        rule.Update(request.Title, request.Description, request.Order);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

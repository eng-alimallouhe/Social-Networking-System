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

namespace SNS.Application.ContentManagement.Communities.Rules.Commands.CreateCommunityRule;

/// <summary>
/// Represents a command to create a new rule within a community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="Title">The title of the rule.</param>
/// <param name="Description">The description of the rule.</param>
/// <param name="Order">The display order of the rule.</param>
public sealed record CreateCommunityRuleCommand(
    Guid CommunityId,
    string Title,
    string Description,
    int Order
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="CreateCommunityRuleCommand"/> to persist a new community rule.
/// </summary>
internal sealed class CreateCommunityRuleCommandHandler : ICommandHandler<CreateCommunityRuleCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<CommunityRule> _ruleRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CreateCommunityRuleCommandHandler(
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

    public async Task<Result> Handle(CreateCommunityRuleCommand request, CancellationToken cancellationToken)
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

        var rule = CommunityRule.Create(request.CommunityId, request.Title, request.Description, request.Order);
        _ruleRepo.Add(rule);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}

using Microsoft.EntityFrameworkCore;
using SNS.Application.ContentManagement.Communities.Rules.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.ContentManagement.Communities.Rules.Queries.GetCommunityRules;

/// <summary>
/// Handles the execution of <see cref="GetCommunityRulesQuery"/> to fetch structured rules for a community.
/// </summary>
internal sealed class GetCommunityRulesQueryHandler : IQueryHandler<GetCommunityRulesQuery, List<CommunityRuleDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetCommunityRulesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<CommunityRuleDto>>> Handle(GetCommunityRulesQuery request, CancellationToken cancellationToken)
    {
        var communityExists = await _dbContext.Communities
            .AnyAsync(c => c.Id == request.CommunityId && c.IsActive, cancellationToken);

        if (!communityExists)
        {
            return Result<List<CommunityRuleDto>>.Failure(ResourceStatusCode.NotFound);
        }

        var rules = await _dbContext.CommunityRules
            .AsNoTracking()
            .Where(r => r.CommunityId == request.CommunityId)
            .OrderBy(r => r.Order)
            .Select(r => new CommunityRuleDto(
                r.Id,
                r.CommunityId,
                r.Title,
                r.Description,
                r.Order))
            .ToListAsync(cancellationToken);

        return Result<List<CommunityRuleDto>>.Success(rules, OperationStatusCode.Success);
    }
}

using SNS.Application.ContentManagement.Communities.Rules.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.ContentManagement.Communities.Rules.Queries.GetCommunityRules;

/// <summary>
/// Represents a query to retrieve all rules configured for a community.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
public sealed record GetCommunityRulesQuery(Guid CommunityId) : IQuery<List<CommunityRuleDto>>;

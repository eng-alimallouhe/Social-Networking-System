namespace SNS.Application.ContentManagement.Communities.Rules.Contracts;

/// <summary>
/// Represents a structured community rule item.
/// </summary>
/// <param name="Id">The unique identifier of the rule.</param>
/// <param name="CommunityId">The unique identifier of the community.</param>
/// <param name="Title">The rule title or headline.</param>
/// <param name="Description">The detailed description of the rule.</param>
/// <param name="Order">The display sort order of the rule.</param>
public sealed record CommunityRuleDto(
    Guid Id,
    Guid CommunityId,
    string Title,
    string Description,
    int Order
);

/// <summary>
/// Represents a payload for creating a new community rule.
/// </summary>
/// <param name="Title">The rule title.</param>
/// <param name="Description">The detailed rule description.</param>
/// <param name="Order">The display order.</param>
public sealed record CreateCommunityRuleDto(
    string Title,
    string Description,
    int Order
);

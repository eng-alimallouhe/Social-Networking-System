using Microsoft.AspNetCore.Http;
using SNS.Application.ContentManagement.Communities.Rules.Contracts;
using SNS.Application.ContentManagement.Communities.Settings.Contracts;
using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.API.Contracts.ContentManagement.Communities;

/// <summary>
/// Represents the multipart form request for creating a new community.
/// </summary>
public sealed record CreateCommunityRequest(
    string Name,
    string Description,
    string RulesText,
    ModerationPolicy Policy,
    CommunityType Type,
    IFormFile? Logo = null,
    CommunitySettingsDto? Settings = null,
    List<CreateCommunityRuleDto>? Rules = null
);

/// <summary>
/// Represents the multipart form request for updating a community.
/// </summary>
public sealed record UpdateCommunityRequest(
    string Name,
    string Description,
    string RulesText,
    ModerationPolicy Policy,
    CommunityType Type,
    CommunityStatus Status,
    IFormFile? Logo = null
);

/// <summary>
/// Represents a request to join a community with optional application message.
/// </summary>
public sealed record JoinCommunityRequest(
    string? Notes = null
);

/// <summary>
/// Represents a request to change a community member's role.
/// </summary>
public sealed record ChangeMemberRoleRequest(
    CommunityRole NewRole
);

/// <summary>
/// Represents a request to update community configuration settings.
/// </summary>
public sealed record UpdateCommunitySettingsRequest(
    bool AllowPostWithoutApproval = true,
    bool AllowInvitationsByMembers = true,
    bool AllowComments = true,
    bool AllowMediaUpload = true
);

/// <summary>
/// Represents a request to create a community rule.
/// </summary>
public sealed record CreateCommunityRuleRequest(
    string Title,
    string Description,
    int Order
);

/// <summary>
/// Represents a request to update an existing community rule.
/// </summary>
public sealed record UpdateCommunityRuleRequest(
    string Title,
    string Description,
    int Order
);

using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.ContentManagement.Communities.Communities.Queries.GetCommunityById;

/// <summary>
/// Represents a query to retrieve detailed community profile information by unique identifier.
/// </summary>
/// <param name="CommunityId">The unique identifier of the community.</param>
public sealed record GetCommunityByIdQuery(Guid CommunityId) : IQuery<CommunityDetailsDto>;

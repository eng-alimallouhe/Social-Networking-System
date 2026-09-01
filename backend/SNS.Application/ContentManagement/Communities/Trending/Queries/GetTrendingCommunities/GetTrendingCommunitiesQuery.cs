using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.ContentManagement.Communities.Trending.Queries.GetTrendingCommunities;

/// <summary>
/// Represents a query to retrieve top trending communities based on activity score.
/// </summary>
/// <param name="Count">The maximum number of trending communities to retrieve.</param>
public sealed record GetTrendingCommunitiesQuery(int Count = 10) : IQuery<List<CommunitySummaryDto>>;

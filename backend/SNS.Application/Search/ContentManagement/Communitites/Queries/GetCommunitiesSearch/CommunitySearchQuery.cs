using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.Search.ContentManagement.Communitites.Queries;

public sealed record CommunitySearchQuery(
    string? SearchTerm = null,
    CommunityType? Type = null, 
    int Page = 1,
    int PageSize = 10);

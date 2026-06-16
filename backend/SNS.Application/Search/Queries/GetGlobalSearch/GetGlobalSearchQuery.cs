using SNS.Application.Search.Queries.GetGlobalSearch;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Search.Queries.GlobalSearch;

public sealed record GetGlobalSearchQuery(
    string SearchTerm,
    int TopResultsPerCategory = 4 
) : IQuery<GlobalSearchResultDto>;

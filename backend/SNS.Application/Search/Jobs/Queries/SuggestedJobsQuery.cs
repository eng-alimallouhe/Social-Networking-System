namespace SNS.Application.Search.Jobs.Queries;

public sealed record SuggestedJobsQuery(
    List<string> Skills,
    List<string> Topics,
    int Page = 1,
    int PageSize = 10
);

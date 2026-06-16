namespace SNS.Application.Search.Projects.Queries;

public sealed record SuggestedProjectsQuery(
    List<string> UserSkills,
    int Page = 1,
    int PageSize = 10
);

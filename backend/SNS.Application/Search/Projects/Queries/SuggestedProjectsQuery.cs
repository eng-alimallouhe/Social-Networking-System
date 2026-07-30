namespace SNS.Application.Search.Projects.Queries;

/// <summary>
/// Represents a query model to retrieve suggested project recommendations based on user skills.
/// </summary>
/// <param name="UserSkills">List of user skills to match against project requirement criteria.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of suggested projects to return per page.</param>
public sealed record SuggestedProjectsQuery(
    List<string> UserSkills,
    int Page = 1,
    int PageSize = 10
);


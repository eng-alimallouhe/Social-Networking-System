namespace SNS.Application.Search.Jobs.Queries;

/// <summary>
/// Represents a query model to retrieve suggested job recommendations based on skills and topics.
/// </summary>
/// <param name="Skills">List of skill names to match against job requirements.</param>
/// <param name="Topics">List of topic names to filter or rank job suggestions.</param>
/// <param name="Page">The page index for pagination (1-based).</param>
/// <param name="PageSize">The maximum number of suggested jobs to return per page.</param>
public sealed record SuggestedJobsQuery(
    List<string> Skills,
    List<string> Topics,
    int Page = 1,
    int PageSize = 10
);


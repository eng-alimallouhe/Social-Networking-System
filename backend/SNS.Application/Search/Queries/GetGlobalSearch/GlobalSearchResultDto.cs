using SNS.Domain.Search.Documents;

namespace SNS.Application.Search.Queries.GetGlobalSearch;

public sealed record GlobalSearchResultDto
{
    public List<ProfileDocument> Profiles { get; init; } = new();
    public List<ProjectDocument> Projects { get; init; } = new();
    public List<CommunityDocument> Communities { get; init; } = new();
    public List<JobsDocument> Jobs { get; init; } = new();
    public List<ProblemDocument> Problems { get; init; } = new();
}

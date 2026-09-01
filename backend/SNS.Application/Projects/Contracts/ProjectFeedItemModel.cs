namespace SNS.Application.Projects.Contracts;

public readonly struct ProjectFeedItemModel
{
    public Guid ProjectId { get; init; }
    public double Score { get; init; }

    public ProjectFeedItemModel(Guid projectId, double score)
    {
        ProjectId = projectId;
        Score = score;
    }
}

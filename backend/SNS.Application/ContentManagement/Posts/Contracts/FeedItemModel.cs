namespace SNS.Application.ContentManagement.Posts.Contracts;

public readonly struct FeedItemModel
{
    public Guid PostId { get; init; }
    public double Score { get; init; }

    public FeedItemModel(Guid postId, double score)
    {
        PostId = postId;
        Score = score;
    }
}
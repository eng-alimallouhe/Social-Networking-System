namespace SNS.Domain.Profiles.Profiles.Constants;

/// <summary>
/// Defines centralized reputation point values for system actions.
/// </summary>
public static class ReputationPointValues
{
    /// <summary>
    /// Points awarded upon user account creation.
    /// </summary>
    public const int AccountCreated = 10;

    /// <summary>
    /// Points awarded when a post is created.
    /// </summary>
    public const int PostCreated = 5;

    /// <summary>
    /// Points deducted when a post is deleted.
    /// </summary>
    public const int PostDeleted = -5;

    /// <summary>
    /// Points awarded when a comment is created.
    /// </summary>
    public const int CommentCreated = 2;

    /// <summary>
    /// Points deducted when a comment is deleted.
    /// </summary>
    public const int CommentDeleted = -2;

    /// <summary>
    /// Points awarded when a resume is created.
    /// </summary>
    public const int ResumeCreated = 5;

    /// <summary>
    /// Points deducted when a resume is deleted.
    /// </summary>
    public const int ResumeDeleted = -5;

    /// <summary>
    /// Points awarded to the post author when a reaction is added to their post.
    /// </summary>
    public const int PostReactionAdded = 2;

    /// <summary>
    /// Points deducted from the post author when a reaction is removed from their post.
    /// </summary>
    public const int PostReactionRemoved = -2;

    /// <summary>
    /// Points awarded to the comment author when a reaction is added to their comment.
    /// </summary>
    public const int CommentReactionAdded = 1;

    /// <summary>
    /// Points deducted from the comment author when a reaction is removed from their comment.
    /// </summary>
    public const int CommentReactionRemoved = -1;

    /// <summary>
    /// Points awarded for receiving a like.
    /// </summary>
    public const int ReceivedLike = 2;

    /// <summary>
    /// Points deducted for receiving a downvote.
    /// </summary>
    public const int ReceivedDownvote = -1;

    /// <summary>
    /// Points awarded when an answer is accepted.
    /// </summary>
    public const int AnswerAccepted = 15;
}

using System.Text.Json.Serialization;

namespace SNS.Domain.Profiles.Profiles.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReputationActionType
{
    AccountCreated = 1,    // +10 points
    CreatedPost = 2,       // +5 points
    ReceivedLike = 3,      // +2 points
    AnswerAccepted = 4,    // +15 points
    PostDeleted = 5,       // -5 points (Penalty)
    ReceivedDownvote = 6,  // -1 point
    CreatedComment = 7,    // +2 points
    CommentDeleted = 8,    // -2 points
    CreatedResume = 9,     // +5 points
    ResumeDeleted = 10,    // -5 points
    PostReactionAdded = 11, // +2 points
    PostReactionRemoved = 12, // -2 points
    CommentReactionAdded = 13, // +1 point
    CommentReactionRemoved = 14 // -1 point
}

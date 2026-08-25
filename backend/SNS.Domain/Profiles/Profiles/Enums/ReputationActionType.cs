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
    ReceivedDownvote = 6   // -1 point
}

using System.Text.Json.Serialization;

namespace SNS.Domain.Moderation.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReportTargetType
{
    Post = 1,
    Comment = 2,
    UserProfile = 3,
    Rating = 4,
    Project = 5,
    Company = 6,
    Job = 7
}

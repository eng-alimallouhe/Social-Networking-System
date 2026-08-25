using System.Text.Json.Serialization;

namespace SNS.Domain.Discussions.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

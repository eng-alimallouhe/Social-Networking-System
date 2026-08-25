using System.Text.Json.Serialization;

namespace SNS.Domain.Preferences.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProficiencyLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Expert
}

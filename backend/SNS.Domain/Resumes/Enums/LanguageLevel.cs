using System.Text.Json.Serialization;

namespace SNS.Domain.Resumes.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LanguageLevel
{
    Beginner,
    Intermediate,
    Advanced,
    Fluent,
    Native
}

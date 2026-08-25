using System.Text.Json.Serialization;

namespace SNS.Domain.Resumes.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Language
{
    Arabic,
    English,
    Turkish,
    Indian,
    German
}

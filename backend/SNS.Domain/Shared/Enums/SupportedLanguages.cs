using System.Text.Json.Serialization;

namespace SNS.Domain.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]

public enum SupportedLanguages
{
    Arabic = 1,
    English = 2,
}

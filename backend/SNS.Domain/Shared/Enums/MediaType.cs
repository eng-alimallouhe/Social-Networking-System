using System.Text.Json.Serialization;

namespace SNS.Domain.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]

public enum MediaType
{
    Image,
    Video,
    Audio
}
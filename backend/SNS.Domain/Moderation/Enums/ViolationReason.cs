using System.Text.Json.Serialization;

namespace SNS.Domain.Moderation.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ViolationReason
{
    Hate_Speech = 0,
    Spam = 1,
}

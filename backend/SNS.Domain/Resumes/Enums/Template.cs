using System.Text.Json.Serialization;

namespace SNS.Domain.Resumes.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Template
{
    Impact,
    Blue,
    Green,
    National
}

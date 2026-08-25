using System.Text.Json.Serialization;

namespace SNS.Domain.Discussions.Solutions.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SolutionBlockType
{
    Text,
    Code,
    Media
}

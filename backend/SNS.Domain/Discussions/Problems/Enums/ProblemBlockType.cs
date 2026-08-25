using System.Text.Json.Serialization;

namespace SNS.Domain.Discussions.Problems.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProblemBlockType
{
    Text,
    Code,
    Image,
    Video
}

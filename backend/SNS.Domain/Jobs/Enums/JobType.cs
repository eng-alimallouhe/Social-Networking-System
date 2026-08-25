using System.Text.Json.Serialization;

namespace SNS.Domain.Jobs.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JobType
{
    FullTime,
    PartTime,
    Internship,
    Contract,
    Remote,
    Hybrid
}

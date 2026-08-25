using System.Text.Json.Serialization;

namespace SNS.Domain.Shared.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TimePeriod
{
    Week = 7,
    Month = 30,
    ThreeMonth = 90,
    Year = 365
}
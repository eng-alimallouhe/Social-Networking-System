using System.Text.Json.Serialization;

namespace SNS.Domain.Jobs.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SalaryType
{
    Monthly,
    Yearly,
    Hourly,
    Negotiable
}

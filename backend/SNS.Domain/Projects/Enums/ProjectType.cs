using System.Text.Json.Serialization;

namespace SNS.Domain.Projects.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProjectType
{
    OpenSource,
    Academic,
    Personal,
    Commercial
}

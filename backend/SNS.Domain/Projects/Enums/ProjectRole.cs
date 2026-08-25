using System.Text.Json.Serialization;

namespace SNS.Domain.Projects.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProjectRole
{
    Developer,
    Designer,
    Tester,
    ProjectManager,
    Other
}

using System.Text.Json.Serialization;

namespace SNS.Domain.ContentManagement.Communities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommunityType
{
    Public,
    Private,
    Restricted
}

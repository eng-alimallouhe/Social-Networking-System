using System.Text.Json.Serialization;

namespace SNS.Domain.ContentManagement.Communities.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommunityActionType
{
    CommunityCreated = 1,
    CommunityUpdated = 2,
    CommunityArchived = 3,
    CommunityRestored = 4,

    MemberJoined = 10,
    MemberLeft = 11,
    MemberRemoved = 12,
    MemberBanned = 13,
    MemberUnbanned = 14,

    RoleAssigned = 20,
    RoleRemoved = 21,

    ModeratorAdded = 30,
    ModeratorRemoved = 31,

    JoinRequestSubmitted = 40,
    JoinRequestApproved = 41,
    JoinRequestRejected = 42,

    RulesUpdated = 50,
    PrivacyChanged = 51,

    PostCreated = 60,
    PostDeleted = 61
}
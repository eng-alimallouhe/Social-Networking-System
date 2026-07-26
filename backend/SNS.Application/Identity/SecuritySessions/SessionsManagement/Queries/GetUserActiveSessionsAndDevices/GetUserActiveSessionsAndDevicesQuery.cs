using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserActiveSessionsAndDevices;

#region 🔍 Query Request

public sealed record GetUserActiveSessionsAndDevicesQuery() : IQuery<UserActiveSessionsAndDevicesResult>;

#endregion
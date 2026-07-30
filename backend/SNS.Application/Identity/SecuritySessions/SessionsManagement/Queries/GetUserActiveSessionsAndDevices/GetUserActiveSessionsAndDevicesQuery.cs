using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserActiveSessionsAndDevices;

#region 🔍 Query Request

/// <summary>
/// Represents a query to retrieve active security sessions and registered devices for the authenticated user.
/// </summary>
public sealed record GetUserActiveSessionsAndDevicesQuery() : IQuery<UserActiveSessionsAndDevicesResult>;

#endregion
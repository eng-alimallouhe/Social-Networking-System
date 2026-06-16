using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.Identity.SecuritySessions.Queries.GetUserSessions;

public sealed record GetUserSessionsQuery(
    bool JustActiveSessions,
    int CurrentPage = 1,
    int PageSize = 10) : IQuery<Paged<SessionSummaryDto>>;

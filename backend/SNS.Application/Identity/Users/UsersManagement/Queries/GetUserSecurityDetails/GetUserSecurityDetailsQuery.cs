using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserSecurityDetails;

/// <summary>
/// Represents a query to retrieve multi-factor authentication and security settings for the authenticated user.
/// </summary>
public sealed record GetUserSecurityDetailsQuery : IQuery<UserSecurityDetailsResult>;


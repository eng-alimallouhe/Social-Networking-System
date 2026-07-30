using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.AdminAcions.Queries.GetUserDetails;

/// <summary>
/// Represents an administrative query to retrieve detailed profile, security session, and metric information for a user.
/// </summary>
/// <param name="TargetUserId">The unique identifier of the user whose details are being requested.</param>
public sealed record GetUserDetailsQuery(Guid TargetUserId) : IQuery<UserDetailsDto>;


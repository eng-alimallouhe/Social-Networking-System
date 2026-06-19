using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.AdminAcions.Queries.GetUserDetails;

public sealed record GetUserDetailsQuery(Guid TargetUserId) : IQuery<UserDetailsDto>;

using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserSecurityDetails;

public sealed record GetUserSecurityDetailsQuery : IQuery<UserSecurityDetailsResult>;

using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserInformation;

public sealed record GetUserInformationQuery : IQuery<UserInformationResult>;

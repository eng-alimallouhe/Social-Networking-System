using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserInformation;

/// <summary>
/// Represents a query to retrieve profile and account summary information for the authenticated user.
/// </summary>
public sealed record GetUserInformationQuery : IQuery<UserInformationResult>;


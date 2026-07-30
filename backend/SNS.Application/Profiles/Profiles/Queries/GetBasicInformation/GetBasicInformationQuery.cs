using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Queries.GetBasicInformation;

/// <summary>
/// Represents a query to retrieve basic profile information for the authenticated user.
/// </summary>
public sealed record GetBasicInformationQuery() : IQuery<BasicInformationDto>;
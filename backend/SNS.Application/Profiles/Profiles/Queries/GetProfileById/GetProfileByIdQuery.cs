using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileById;

/// <summary>
/// Represents a query to retrieve detailed profile information for a specific profile ID.
/// </summary>
/// <param name="profileId">The unique identifier of the target profile to retrieve.</param>
public sealed record GetProfileByIdQuery(Guid profileId) : IQuery<ProfileDetailsDto>;


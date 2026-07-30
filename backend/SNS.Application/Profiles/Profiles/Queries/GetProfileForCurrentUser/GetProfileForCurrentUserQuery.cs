using MediatR;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Shared.Results;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileForCurrentUser;

/// <summary>
/// Represents a query to retrieve base profile details for the currently authenticated user.
/// </summary>
public sealed record GetProfileForCurrentUserQuery() : IRequest<Result<ProfileBaseDto>>;


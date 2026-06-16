using MediatR;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Shared.Results;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileForCurrentUser;

public sealed record GetProfileForCurrentUserQuery() : IRequest<Result<ProfileBaseDto>>;

using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileById;

public sealed record GetProfileByIdQuery(Guid profileId) : IQuery<ProfileDetailsDto>;

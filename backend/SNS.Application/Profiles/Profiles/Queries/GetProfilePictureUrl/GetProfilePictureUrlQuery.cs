using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfilePictureUrl;

public sealed record GetProfilePictureUrlQuery(): IQuery<string>;
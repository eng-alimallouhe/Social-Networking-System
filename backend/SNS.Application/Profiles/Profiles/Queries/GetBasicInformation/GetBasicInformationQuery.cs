using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Queries.GetBasicInformation;

public sealed record GetBasicInformationQuery() : IQuery<BasicInformationDto>;